using System.Collections;
using UnityEngine;

public class CrossAttack : MonoBehaviour
{
    [Header("Cross arms (children with PossessedEveGroundDamageZone)")]
    [SerializeField] Transform armVertical;
    [SerializeField] Transform armHorizontal;
    [Header("Sizing")]
    [SerializeField] float armLength = 8f;
    [SerializeField] float armThickness = 1f;
    [Header("Timing")]
    [SerializeField] float earlyTelegraphForSpiritualSeconds = 1.5f;
    [SerializeField] float telegraphForEveryoneSeconds = 1f;
    [SerializeField] float activeDurationSeconds = 6f;
    [Header("Damage")]
    [SerializeField] int damagePerTick = 15;
    [SerializeField] float tickInterval = 0.2f;
    [Header("Rotation")]
    [SerializeField] float rotationTurns = 3f;
    PossessedEveGroundDamageZone _vZone;
    PossessedEveGroundDamageZone _hZone;



    public bool Finished { get; private set; }
    void Awake()
    {
        _vZone = armVertical.GetComponent<PossessedEveGroundDamageZone>();
        _hZone = armHorizontal.GetComponent<PossessedEveGroundDamageZone>();
    }
    public void Begin(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        Finished = false;
        StartCoroutine(Run());
    }
    IEnumerator Run()
    {
        Vector2 size = new Vector2(armLength, armThickness);
        _vZone.Configure(size, damagePerTick, tickInterval, earlyTelegraphForSpiritualSeconds + telegraphForEveryoneSeconds + activeDurationSeconds + 0.5f);
        _hZone.Configure(size, damagePerTick, tickInterval, earlyTelegraphForSpiritualSeconds + telegraphForEveryoneSeconds + activeDurationSeconds + 0.5f);
        _vZone.SetArmed(false);
        _hZone.SetArmed(false);
        float t = 0f;
        while (t < earlyTelegraphForSpiritualSeconds)
        {
            t += Time.deltaTime;
            bool spirit = PlayerController.instance != null && PlayerController.instance.IsInSpiritualVision();
            float alpha = spirit ? 1f : 0.15f;
            _vZone.SetVisualAlpha(alpha);
            _hZone.SetVisualAlpha(alpha);
            yield return null;
        }
        t = 0f;
        while (t < telegraphForEveryoneSeconds)
        {
            t += Time.deltaTime;
            _vZone.SetVisualAlpha(0.7f);
            _hZone.SetVisualAlpha(0.7f);
            yield return null;
        }
        _vZone.SetArmed(true);
        _hZone.SetArmed(true);
        float activeT = 0f;
        while (activeT < activeDurationSeconds)
        {
            activeT += Time.deltaTime;
            float u = activeDurationSeconds > 0f ? activeT / activeDurationSeconds : 1f;
            float speedMul = Mathf.Lerp(0.5f, 2f, u);
            float degrees = (360f * rotationTurns * speedMul / activeDurationSeconds) * Time.deltaTime;
            transform.Rotate(0f, 0f, degrees);
            yield return null;
        }
        Finished = true;
    }
}
