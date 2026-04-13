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

    [Header("Attack 3 — Vatican Guard")]
    [SerializeField] GameObject[] vaticanKnightPrefabs; 
    [SerializeField] int vaticanKnightsPerType = 1;
    [SerializeField] int vaticanTotalCap = 12;
    [SerializeField] float vaticanSummonRandomRadius = 5f;
    [SerializeField] float vaticanPhaseDuration = 45f;
    [SerializeField] float vaticanBaseResurrectInterval = 8f;
    [SerializeField] float exploitResurrectIntervalReduction = 1f;
    [SerializeField] float exploitResurrectedHpLossPerStack = 0.15f;
    [SerializeField] Transform[] vaticanSpawnPoints;
    struct VaticanKnightSlot
    {
        public StandardEnemyBase Enemy;
        public int Kind; // 0 shield, 1 blade, 2 hammer
    }
    readonly List<VaticanKnightSlot> vaticanSlots = new();
    Coroutine vaticanAttackRoutine;
    int vaticanExploitStacks;



    public void NotifyVaticanExploitSuccess()
    {
        vaticanExploitStacks++;
    }
    float VaticanResurrectInterval =>
        Mathf.Max(0.5f, vaticanBaseResurrectInterval - vaticanExploitStacks * exploitResurrectIntervalReduction);
    float VaticanResurrectedHpMultiplier =>
        Mathf.Max(0.2f, 1f - vaticanExploitStacks * exploitResurrectedHpLossPerStack);

    private void Awake()
    {
        cooldown_time = 3f;
        default_enemy_state = new Boss_Cooldown(1f);
        attackProbalities = new float[5] { 50f, 50f, 0f, 0f, 0f };


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
        throw new System.NotImplementedException();
    }

    public override void Attack4()
    {
        throw new System.NotImplementedException();
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
        return -1; // everyone at quota (or no prefabs)
    }
    void SpawnVaticanKnight(int kind, int posIndex, bool resurrection)
    {
        GameObject p = Prefab(kind);
        if (p == null) return;
        StandardEnemyBase e = BossSummonUtility.SpawnBossMinion(p, VaticanPos(posIndex));
        if (e == null) return;
        if (resurrection && VaticanResurrectedHpMultiplier < 0.999f)
            e.health = Mathf.Max(1, Mathf.RoundToInt(e.health * VaticanResurrectedHpMultiplier));
        vaticanSlots.Add(new VaticanKnightSlot { Enemy = e, Kind = kind });
    }
    IEnumerator VaticanGuardAttack()
    {
        isAttacking = true;
        vaticanSlots.Clear();
        int pos = 0;
        for (int t = 0; t < vaticanKnightsPerType; t++)
            for (int k = 0; k < 3; k++)
            {
                RefreshVaticanRoster(out int alive, out _, out _, out _);
                if (alive >= vaticanTotalCap) break;
                if (Prefab(k) != null)
                    SpawnVaticanKnight(k, pos++, false);
            }
        RefreshVaticanRoster(out int initialTarget, out _, out _, out _);
        float nextResurrect = Time.time + VaticanResurrectInterval;
        float endTime = Time.time + Mathf.Max(0f, vaticanPhaseDuration);
        while (Time.time < endTime)
        {
            RefreshVaticanRoster(out int alive, out int c0, out int c1, out int c2);
            if (alive < initialTarget && alive < vaticanTotalCap && Time.time >= nextResurrect)
            {
                int k = FirstKindBelowQuota(vaticanKnightsPerType, c0, c1, c2);
                if (k >= 0)
                {
                    SpawnVaticanKnight(k, pos++, true);
                    nextResurrect = Time.time + VaticanResurrectInterval;
                }
            }
            yield return null;
        }
        cooldown_time = 5f;
        isAttacking = false;
        vaticanAttackRoutine = null;
    }
}
