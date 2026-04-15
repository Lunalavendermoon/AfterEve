using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessedEvePhase1 : BossBehaviourBase
{

    [SerializeField] CrossAttack holyRadiancePrefab;
    [SerializeField] GameObject wrathMarkPrefab;   // has PossessedEveWrathMark
    [SerializeField] LightningStrike lightningPrefab;
    [SerializeField] int wrathBoltCount = 5;

    const int VaticanPerType = 2;
    const int VaticanTotal = 6;
    int forcedNextAttackNumber = -1; 
    [SerializeField] float comboGapAfterVaticanBeforeSacredWarSeconds = 5f;

    [Header("Attack 3 — Vatican Guard")]

    [SerializeField] GameObject[] vaticanKnightPrefabs = new GameObject[3];
    [SerializeField] float vaticanSummonRandomRadius = 5f;
    [SerializeField] float vaticanReviveIntervalSeconds = 3f;
    [SerializeField] Transform[] vaticanSpawnPoints;
    [Header("Vatican — exploit (narrative)")]

    [SerializeField] float exploitReviveIntervalMultiplier = 0.25f;
    [SerializeField] float exploitResurrectedHpMultiplier = 0.25f;
    struct VaticanKnightSlot
    {
        public StandardEnemyBase Enemy;
        public int Kind;
    }
    readonly List<VaticanKnightSlot> vaticanSlots = new();
    Coroutine vaticanAttackRoutine;
    int vaticanExploitStacks;


    [Header("Attack 4 — Sacred War")]
    public static bool SacredWarNarrativeComplete { get; set; }
    [Tooltip("Prefab variants [Shield, Blade, Hammer] with SacredWarKnightBehaviour + trigger collider.")]
    [SerializeField] GameObject[] sacredWarKnightPrefabs = new GameObject[3];
    [SerializeField] int sacredWarShieldBonus = 100;
    [SerializeField] float sacredWarContactDpsAfterNarrative = 12f;
    [SerializeField] float sacredWarContactDpsBeforeNarrative = 400f;
    [SerializeField] float sacredWarMarchSpeed = 2f;
    [SerializeField] float sacredWarMaxDuration = 25f;
    [SerializeField] float sacredWarEdgeMargin = 0.75f;

    bool sacredWarActive;
    readonly List<SacredWarKnightBehaviour> sacredWarUnits = new();
    public void NotifyVaticanExploitSuccess() => vaticanExploitStacks++;
    float EffectiveVaticanReviveInterval
    {
        get
        {
            float v = vaticanExploitStacks > 0
                ? Mathf.Max(0.1f, vaticanReviveIntervalSeconds * exploitReviveIntervalMultiplier)
                : vaticanReviveIntervalSeconds;
            if (sacredWarActive)
                v *= 0.5f;
            return v;
        }
    }
    private void Awake()
    {
        cooldown_time = 3f;
        default_enemy_state = new Boss_Cooldown(1f);
        attackProbalities = new float[5] { 0f, 0f, 100f, 0f, 0f };


    }

    public override int ChooseAttack()
    {
        if (forcedNextAttackNumber > 0)
        {
            int n = forcedNextAttackNumber;
            forcedNextAttackNumber = -1;
            return n;
        }
        RefreshVaticanRoster(out int alive, out _, out _, out _);
        float choice = Random.Range(0f, 1f) * 100f;
        float sum = 0f;
        for (int i = 0; i < attackProbalities.Length; i++)
        {
            float w = attackProbalities[i];
            if (i == 2 && alive >= VaticanTotal)
                w = 0f;
            if (i == 3)
                w = 0f; // 4 never random; only after 3
            if (w <= 0f) continue;
            sum += w;
            if (choice <= sum)
                return i + 1;
        }
        return 1;
    }
    public override void BossUpdate()
    {
        base.BossUpdate();

    }
    public override void Attack1()
    {
        StartCoroutine(HolyRadianceAttack());
    }

    public override void Attack2()
    {
        StartCoroutine(DivineWrathAttack());
    }

    public override void Attack3()
    {
        if (vaticanAttackRoutine != null)
            StopCoroutine(vaticanAttackRoutine);
        vaticanAttackRoutine = StartCoroutine(VaticanGuardAttack());
    }

    public override void Attack4()
    {
        StartCoroutine(SacredWarAttack());
    }

    public override void Attack5()
    {
        throw new System.NotImplementedException();
    }

    public override void Movement()
    {
        Vector2 nextPoint= FindNextTeleport();
        transform.position = new Vector3(nextPoint.x, nextPoint.y, transform.position.z);

    }

    private Vector2 FindNextTeleport()
    {
        const float teleportRadius = 10f;
        const int attempts = 100;
        Vector3 chosen = transform.position;
        bool found = false;
        Vector2 randomOffset = Random.insideUnitCircle * teleportRadius;
        Vector2 candidate2D = new Vector2(transform.position.x, transform.position.y) + randomOffset;
        for (int i = 0; i < attempts; i++)
        {
            randomOffset = Random.insideUnitCircle * teleportRadius;
            candidate2D = new Vector2(transform.position.x, transform.position.y) + randomOffset;
            bool inside = false;
            // Narrative room
            if (NarrativeRoomManager.instance != null &&
                NarrativeRoomManager.instance.IsPointInsideCurrentRoom(candidate2D, 0f))
            {
                inside = true;
            }
            else if (GameManager.instance != null &&
                     GameManager.instance.IsWorldPointInsideMap(candidate2D, 0f))
            {
                inside = true;
            }
            if (inside)
            {
                chosen = new Vector3(candidate2D.x, candidate2D.y, transform.position.z);
                found = true;
                break;
            }
        }
        if (!found) return transform.position; 
        return candidate2D;
    }

    IEnumerator HolyRadianceAttack()
    {
        isAttacking = true;
        Vector3 center = PlayerController.instance != null
            ? PlayerController.instance.transform.position
            : transform.position;
        var cross = Instantiate(holyRadiancePrefab, center, Quaternion.identity);
        cross.Begin(center);
        yield return new WaitUntil(() => cross.Finished);
        isAttacking = false;
    }

    IEnumerator DivineWrathAttack()
    {
        isAttacking = true;
        for (int i = 0; i < wrathBoltCount; i++)
        {
            Vector2 p = RandomStrikePoint();
            var markGo = Instantiate(wrathMarkPrefab, p, Quaternion.identity);
            var mark = markGo.GetComponent<WrathMark>();
            bool fired = false;
            mark.Begin(() => { fired = true; });
            yield return new WaitUntil(() => fired);
            Instantiate(lightningPrefab, p, Quaternion.identity);
            Destroy(markGo, 0.1f);
        }
        isAttacking = false;
    }

    #region Attack 3 — Vatican Guard
    Vector2 RandomStrikePoint()
    {

        for (int attempt = 0; attempt < 40; attempt++)
        {
            Vector2 c = PlayerController.instance != null
                ? (Vector2)PlayerController.instance.transform.position + Random.insideUnitCircle * 6f
                : (Vector2)transform.position;
            if (NarrativeRoomManager.instance != null &&
                NarrativeRoomManager.instance.IsPointInsideCurrentRoom(c, 0f))
                return c;
            if (GameManager.instance != null && GameManager.instance.IsWorldPointInsideMap(c, 0f))
                return c;
        }
        return PlayerController.instance != null
            ? (Vector2)PlayerController.instance.transform.position
            : (Vector2)transform.position;
    }

    GameObject Prefab(int k) =>
    vaticanKnightPrefabs != null && k >= 0 && k < vaticanKnightPrefabs.Length
        ? vaticanKnightPrefabs[k]
        : null;
    Vector3 VaticanPos(int i)
    {
        float z = transform.position.z;
        if (vaticanSpawnPoints != null && vaticanSpawnPoints.Length > 0)
        {
            Transform t = vaticanSpawnPoints[i % vaticanSpawnPoints.Length];
            if (t != null) return t.position;
        }
        return BossSummonUtility.TryGetRandomSpawnPosition(transform.position, vaticanSummonRandomRadius, z, out Vector3 p)
            ? p
            : transform.position;
    }
    void RefreshVaticanRoster(out int aliveTotal, out int alive0, out int alive1, out int alive2)
    {
        aliveTotal = alive0 = alive1 = alive2 = 0;
        for (int i = vaticanSlots.Count - 1; i >= 0; i--)
        {
            var s = vaticanSlots[i];
            if (s.Enemy == null || s.Enemy.health <= 0)
            {
                vaticanSlots.RemoveAt(i);
                continue;
            }
            aliveTotal++;
            if (s.Kind == 0) alive0++;
            else if (s.Kind == 1) alive1++;
            else if (s.Kind == 2) alive2++;
        }
    }
    int FirstKindBelowQuota(int perType, int c0, int c1, int c2)
    {
        for (int k = 0; k < 3; k++)
        {
            if (Prefab(k) == null) continue;
            int c = k == 0 ? c0 : k == 1 ? c1 : c2;
            if (c < perType) return k;
        }
        return -1;
    }
    void SpawnVaticanKnight(int kind, int posIndex, bool isResurrection)
    {
        GameObject p = Prefab(kind);
        if (p == null) return;
        StandardEnemyBase e = BossSummonUtility.SpawnBossMinion(p, VaticanPos(posIndex));
        if (e == null) return;
        if (isResurrection && vaticanExploitStacks > 0 && exploitResurrectedHpMultiplier < 0.999f)
            e.health = Mathf.Max(1, Mathf.RoundToInt(e.health * exploitResurrectedHpMultiplier));
        vaticanSlots.Add(new VaticanKnightSlot { Enemy = e, Kind = kind });
    }
    IEnumerator VaticanGuardAttack()
    {
        isAttacking = true;
        RefreshVaticanRoster(out int startAlive, out int c0, out int c1, out int c2);
        if (startAlive >= VaticanTotal)
        {
            isAttacking = false;
            vaticanAttackRoutine = null;
            yield break;
        }
        int pos = 0;
        if (startAlive == 0)
        {
            for (int t = 0; t < VaticanPerType; t++)
                for (int k = 0; k < 3; k++)
                {
                    if (Prefab(k) == null) continue;
                    SpawnVaticanKnight(k, pos++, false);
                }
        }
        else
        {
            int reviveBudget = VaticanTotal - startAlive;
            float interval = EffectiveVaticanReviveInterval;
            for (int i = 0; i < reviveBudget; i++)
            {
                if (i > 0)
                    yield return new WaitForSeconds(interval);
                RefreshVaticanRoster(out int aliveNow, out c0, out c1, out c2);
                if (aliveNow >= VaticanTotal)
                    break;
                int kind = FirstKindBelowQuota(VaticanPerType, c0, c1, c2);
                if (kind < 0)
                    break;
                SpawnVaticanKnight(kind, pos++, true);
            }
        }
        forcedNextAttackNumber = 4;
        cooldown_time = comboGapAfterVaticanBeforeSacredWarSeconds;
        isAttacking = false;
        vaticanAttackRoutine = null;
    }
    #endregion

    #region Attack 4 — Sacred War
    IEnumerator SacredWarAttack()
    {
        isAttacking = true;
        RefreshVaticanRoster(out int alive, out _, out _, out _);
        if (alive < 3)
        {
            isAttacking = false;
            yield break;
        }
        var snapshot = new List<VaticanKnightSlot>();
        foreach (VaticanKnightSlot s in vaticanSlots)
        {
            if (s.Enemy != null && s.Enemy.health > 0)
                snapshot.Add(s);
        }
        if (snapshot.Count < 3)
        {
            isAttacking = false;
            yield break;
        }
        if (!TryGetSacredWarArenaBounds(out Bounds arena))
        {
            isAttacking = false;
            yield break;
        }
        sacredWarActive = true;
        sacredWarUnits.Clear();
        vaticanSlots.Clear();
        float z = transform.position.z;
        Vector2 playerXY = PlayerController.instance != null
            ? (Vector2)PlayerController.instance.transform.position
            : (Vector2)transform.position;
        float minX = arena.min.x + sacredWarEdgeMargin;
        float maxX = arena.max.x - sacredWarEdgeMargin;
        float minY = arena.min.y + sacredWarEdgeMargin;
        float maxY = arena.max.y - sacredWarEdgeMargin;
        float centerY = (minY + maxY) * 0.5f;
        float wallY = playerXY.y >= centerY ? minY : maxY;
        Vector2 wallNormalTowardPlayer = playerXY.y >= centerY ? Vector2.up : Vector2.down;
        int n = snapshot.Count;
        float dps = SacredWarNarrativeComplete ? sacredWarContactDpsAfterNarrative : sacredWarContactDpsBeforeNarrative;
        for (int i = 0; i < n; i++)
        {
            VaticanKnightSlot slot = snapshot[i];
            float t = n <= 1 ? 0.5f : i / (float)(n - 1);
            float x = Mathf.Lerp(minX, maxX, t);
            Vector3 spawnPos = new Vector3(x, wallY, z);
            GameObject prefab = slot.Kind >= 0 && slot.Kind < sacredWarKnightPrefabs.Length
                ? sacredWarKnightPrefabs[slot.Kind]
                : null;
            if (prefab == null)
                continue;
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            BossSummonUtility.ConfigureAsBossSummon(go.GetComponent<StandardEnemyBase>());
            StandardEnemyBase ne = go.GetComponent<StandardEnemyBase>();
            if (ne != null)
                ne.health = slot.Enemy.health;
            SacredWarKnightBehaviour sw = go.GetComponent<SacredWarKnightBehaviour>();
            if (sw != null)
            {
                sw.ConfigureSacredWar(sacredWarShieldBonus, dps);
                sacredWarUnits.Add(sw);
            }
            Destroy(slot.Enemy.gameObject);
        }
        float elapsed = 0f;
        while (elapsed < sacredWarMaxDuration && sacredWarUnits.Count > 0)
        {
            for (int i = sacredWarUnits.Count - 1; i >= 0; i--)
            {
                if (sacredWarUnits[i] == null)
                {
                    sacredWarUnits.RemoveAt(i);
                    continue;
                }
                StandardEnemyBase e = sacredWarUnits[i].GetComponent<StandardEnemyBase>();
                if (e == null || e.health <= 0)
                {
                    sacredWarUnits.RemoveAt(i);
                    continue;
                }
                Vector2 p = sacredWarUnits[i].transform.position;
                p += wallNormalTowardPlayer * (sacredWarMarchSpeed * Time.deltaTime);
                p.x = Mathf.Clamp(p.x, minX, maxX);
                p.y = Mathf.Clamp(p.y, minY, maxY);
                sacredWarUnits[i].SetWallPosition(new Vector3(p.x, p.y, z));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        foreach (SacredWarKnightBehaviour u in sacredWarUnits)
        {
            if (u != null)
                Destroy(u.gameObject);
        }
        sacredWarUnits.Clear();
        sacredWarActive = false;
        cooldown_time = 8f;
        isAttacking = false;
    }
    bool TryGetSacredWarArenaBounds(out Bounds bounds)
    {
        if (NarrativeRoomManager.instance != null &&
            NarrativeRoomManager.instance.TryGetCurrentRoomWorldBounds(out bounds))
            return true;
        float z = transform.position.z;
        Vector2 c = transform.position;
        if (PlayerController.instance != null)
            c = ((Vector2)transform.position + (Vector2)PlayerController.instance.transform.position) * 0.5f;
        bounds = new Bounds(new Vector3(c.x, c.y, z), new Vector3(24f, 18f, 1f));
        return true;
    }
    #endregion
}
