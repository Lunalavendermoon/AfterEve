using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class PossessedEvePhase1 : BossBehaviourBase
{

    [SerializeField] CrossAttack holyRadiancePrefab;
    [SerializeField] GameObject wrathMarkPrefab;   // has PossessedEveWrathMark
    [SerializeField] LightningStrike lightningPrefab;
    [SerializeField] int wrathBoltCount = 5;
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

}
