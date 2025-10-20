using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleLevelGenerator : MonoBehaviour
{
    [Header("����⣨�� Cross_16 / URoom_16 / T_16 ... �Ͻ�����")]
    public List<RoomBlock> library = new();

    [Header("��ͼ����")]
    [Range(1, 64)] public int desiredRooms = 10;
    public int seed = 12345;

    [Header("��������ײ")]
    [Tooltip("�Ŷ���������ӷ�������һ�㣬���⸡�������ص�")]
    public float pushEpsilon = 0.02f;
    [Tooltip("ֻ��������ĸ��У������ÿ����������õ� RoomBlock ��")]
    public LayerMask roomBlockMask;

    [Header("��������������ñ����壩")]
    public Transform root;

    // ��ʱͣ�ŵ㣨��ֹ��ʵ����û���������ʼ���ص���
    static readonly Vector3 kTempPos = new Vector3(99999, 99999, 99999);

    System.Random rng;

    void Reset() { root = transform; }
    void Awake() { if (!root) root = transform; }

    // ���� ������/������ϵͳ�� R ������ ����
    void Update()
    {
        bool pressed = false;
#if !ENABLE_INPUT_SYSTEM
        pressed = Input.GetKeyDown(KeyCode.R);
#endif
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            pressed |= Keyboard.current.rKey.wasPressedThisFrame;
#endif
        if (pressed) Regenerate();
    }

    [ContextMenu("Regenerate Level")]
    public void Regenerate()
    {
        if (library == null || library.Count == 0)
        {
            Debug.LogWarning("[ProcGen] Library is empty.");
            return;
        }

        // ����ǰУ��һ�Σ�ֻ��ӡ�����޸ģ�
        ValidateLibrary(false, 0.05f);

        ClearChildren(root);
        rng = new System.Random(seed);

        // 1) ��ʼ��
        var firstPrefab = library[rng.Next(library.Count)];
        var first = Instantiate(firstPrefab, root);
        first.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // 2) frontier ��չ
        var frontier = new Queue<(RoomBlock room, Socket socket)>();
        foreach (var s in GetDoorSockets(first)) frontier.Enqueue((first, s));

        int placed = 1;
        int safety = 2048;

        while (frontier.Count > 0 && placed < desiredRooms && safety-- > 0)
        {
            var (aRoom, aOut) = frontier.Dequeue();
            var need = Opposite(aOut.dir);
            var candidates = GetCandidates(need);
            if (candidates.Count == 0) continue;

            bool attached = false;
            for (int tries = 0; tries < 16 && !attached; tries++)
            {
                var pf = candidates[rng.Next(candidates.Count)];
                var b = Instantiate(pf, root);

                // ��ͣ��ײ + ��Զ
                var bRootBox = EnsureRootBox(b);
                bool prevEnabled = bRootBox.enabled;
                bRootBox.enabled = false;
                b.transform.SetPositionAndRotation(kTempPos, Quaternion.identity);

                if (TryAttach(aRoom, aOut, b, bRootBox, out var diag))
                {
                    placed++;
                    attached = true;

                    // ��� b ��������
                    foreach (var s in GetDoorSockets(b))
                    {
                        if (s.dir == need &&
                            (s.transform.position - diag.aOutWorld).sqrMagnitude < 0.0001f)
                            continue; // �����ӵ�������
                        frontier.Enqueue((b, s));
                    }
                }
                else
                {
#if UNITY_EDITOR
                    DestroyImmediate(b.gameObject);
#else
                    Destroy(b.gameObject);
#endif
                }

                if (b && bRootBox) bRootBox.enabled = prevEnabled;
            }
        }

        Debug.Log($"[ProcGen] Placed {placed} rooms.");
    }

    // ���� ���� + ���� + ֻ�á����С����ص���� ����
    struct AttachDiag { public Vector3 aOutWorld, bInWorld; public Vector3 aBoxSize, bBoxSize; }

    bool TryAttach(RoomBlock a, Socket aOut, RoomBlock b, BoxCollider bRootBox, out AttachDiag info)
    {
        info = default;
        var need = Opposite(aOut.dir);
        var bIn = FindSocket(b, need);
        if (!bIn) { Debug.LogWarning($"[{b.name}] missing socket {need}"); return false; }

        // ��ת���� bIn.forward ָ�� -aOut.forward
        Vector3 targetFwd = -aOut.transform.forward;
        var rot = Quaternion.FromToRotation(bIn.transform.forward, targetFwd);
        b.transform.rotation = rot * b.transform.rotation;

        // ƽ�ƣ����Ķ��� + ΢����
        Vector3 delta = aOut.transform.position - bIn.transform.position;
        b.transform.position += delta + targetFwd * pushEpsilon;

        // ������ɺ��ٿ������� �� ���ص����
        bRootBox.enabled = true;

        if (IsOverlappingRootBox(b, bRootBox))
        {
            var aBox = EnsureRootBox(a);
            info = new AttachDiag
            {
                aOutWorld = aOut.transform.position,
                bInWorld = bIn.transform.position,
                aBoxSize = aBox.size,
                bBoxSize = bRootBox.size
            };
            Debug.LogWarning(
                $"Overlap detected. Check collider size or socket positions.\n" +
                $"A:{a.name} pos={a.transform.position} box={aBox.size}\n" +
                $"B:{b.name} pos={b.transform.position} box={bRootBox.size}\n" +
                $"AOut={aOut.transform.position}  BIn={bIn.transform.position}"
            );
            return false;
        }
        return true;
    }

    BoxCollider EnsureRootBox(RoomBlock rb)
    {
        // ���� RoomBlock.bounds Ϊ Collider/BoxCollider
        BoxCollider bc = null;
        if (rb.bounds != null) bc = rb.bounds as BoxCollider;
        if (bc == null) bc = rb.GetComponent<BoxCollider>();
        if (bc == null) bc = rb.gameObject.AddComponent<BoxCollider>();
        rb.bounds = bc;  // �� RoomBlock ��ס�������
        return bc;
    }

    bool IsOverlappingRootBox(RoomBlock b, BoxCollider bRootBox)
    {
        Vector3 c = bRootBox.bounds.center;
        Vector3 e = bRootBox.bounds.extents;

        Collider[] hits = (roomBlockMask.value != 0)
            ? Physics.OverlapBox(c, e, Quaternion.identity, roomBlockMask, QueryTriggerInteraction.Ignore)
            : Physics.OverlapBox(c, e, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

        foreach (var h in hits)
        {
            if (!h) continue;
            if (h == bRootBox) continue;

            var otherRB = h.GetComponentInParent<RoomBlock>();
            if (otherRB && otherRB.transform != b.transform)
            {
                var otherRoot = otherRB.bounds as Collider;
                if (otherRoot != null && ReferenceEquals(h, otherRoot))
                    return true;    // �����롰��ķ�����С��ཻ
            }
        }
        return false;
    }

    // ���� ��У�飺�гߴ��Ƿ���С��Socket �Ƿ��� ��8 �ߣ������ȵ� ��8/��4�� ����
    [ContextMenu("Validate Library (Log Only)")]
    public void ValidateLibraryLogOnly() => ValidateLibrary(false, 0.05f);

    void ValidateLibrary(bool autoSnap, float snapTol)
    {
        foreach (var pf in library)
        {
            if (!pf) continue;

            // ��ʱʵ�����ڶ��ֲ����꣨Prefab Asset ����ֱ�Ӷ���
#if UNITY_EDITOR
            var inst = (RoomBlock)PrefabUtility.InstantiatePrefab(pf);
#else
            var inst = Instantiate(pf);
#endif
            inst.transform.SetPositionAndRotation(kTempPos, Quaternion.identity);

            var box = EnsureRootBox(inst);
            var size = box.size;

            // 1) ���м�飨������С��ռ�أ�
            if (Mathf.Approximately(size.x, 16f) || Mathf.Approximately(size.z, 16f))
                Debug.LogWarning($"[Validate] {pf.name} root BoxCollider is full size {size}. Use ~15.6~15.8.");
            else
                Debug.Log($"[Validate] {pf.name} root box={size}");

            // 2) Socket ���
            foreach (var s in inst.GetComponentsInChildren<Socket>(true))
            {
                if (s.kind != Socket.Kind.Door) continue;

                var lp = s.transform.localPosition;
                bool ok = true;
                float wantX = 0, wantZ = 0;

                switch (s.dir)
                {
                    case Socket.Dir.N: ok = Mathf.Abs(lp.x) < 0.01f && Nearly(lp.z, +8f, snapTol, out wantZ); break;
                    case Socket.Dir.S: ok = Mathf.Abs(lp.x) < 0.01f && Nearly(lp.z, -8f, snapTol, out wantZ); break;
                    case Socket.Dir.E: ok = Mathf.Abs(lp.z) < 0.01f && Nearly(lp.x, +8f, snapTol, out wantX); break;
                    case Socket.Dir.W: ok = Mathf.Abs(lp.z) < 0.01f && Nearly(lp.x, -8f, snapTol, out wantX); break;
                }

                if (!ok)
                {
                    Debug.LogWarning($"[Validate] {pf.name}/{s.name} dir={s.dir} localPos={lp} EXPECT ��8 on the correct axis.");
                    if (autoSnap)
                    {
                        var p = lp;
                        if (s.dir == Socket.Dir.N || s.dir == Socket.Dir.S) p = new Vector3(0f, lp.y, wantZ);
                        else p = new Vector3(wantX, lp.y, 0f);
                        s.transform.localPosition = p;
                        Debug.Log($"[Snap] {pf.name}/{s.name} -> {p}");
                    }
                }
            }

#if UNITY_EDITOR
            // �ѿ��ܵ������޸� Apply �� prefab
            if (autoSnap)
            {
                var asset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(inst);
                if (asset) PrefabUtility.ApplyPrefabInstance(inst.gameObject, InteractionMode.AutomatedAction);
            }
            DestroyImmediate(inst.gameObject);
#else
            Destroy(inst.gameObject);
#endif
        }
    }

    // ���� һ����У�鲢������ ��8�� �������༭���˵���
#if UNITY_EDITOR
    [MenuItem("Tools/ProcGen/Validate & Snap Sockets")]
    public static void Menu_ValidateAndSnap()
    {
        var gen = FindObjectOfType<SimpleLevelGenerator>();
        if (!gen) { Debug.LogWarning("No SimpleLevelGenerator in scene."); return; }
        gen.ValidateLibrary(true, 0.15f); // ���� 0.15 ��������ֵ
        Debug.Log("[ProcGen] Validate & Snap finished.");
    }
#endif

    // ���� ���� ����
    IEnumerable<Socket> GetDoorSockets(RoomBlock rb)
    {
        foreach (var s in rb.GetComponentsInChildren<Socket>(true))
            if (s.kind == Socket.Kind.Door) yield return s;
    }

    List<RoomBlock> GetCandidates(Socket.Dir needDir)
    {
        var list = new List<RoomBlock>();
        foreach (var pf in library)
        {
            if (!pf) continue;
            var sockets = pf.GetComponentsInChildren<Socket>(true);
            foreach (var s in sockets)
            {
                if (s.kind == Socket.Kind.Door && s.dir == needDir) { list.Add(pf); break; }
            }
        }
        return list;
    }

    Socket FindSocket(RoomBlock rb, Socket.Dir dir)
    {
        foreach (var s in rb.GetComponentsInChildren<Socket>(true))
            if (s.kind == Socket.Kind.Door && s.dir == dir) return s;
        return null;
    }

    Socket.Dir Opposite(Socket.Dir d) =>
        d == Socket.Dir.N ? Socket.Dir.S :
        d == Socket.Dir.S ? Socket.Dir.N :
        d == Socket.Dir.E ? Socket.Dir.W : Socket.Dir.E;

    void ClearChildren(Transform t)
    {
        var list = new List<GameObject>();
        foreach (Transform c in t) list.Add(c.gameObject);
        foreach (var go in list)
        {
#if UNITY_EDITOR
            if (Application.isPlaying) Destroy(go);
            else DestroyImmediate(go);
#else
            Destroy(go);
#endif
        }
    }

    static bool Nearly(float v, float target, float tol, out float snapped)
    {
        snapped = target;
        return Mathf.Abs(v - target) <= tol;
    }
}
