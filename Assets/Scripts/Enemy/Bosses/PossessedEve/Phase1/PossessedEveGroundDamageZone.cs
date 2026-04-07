using UnityEngine;

public class PossessedEveGroundDamageZone : MonoBehaviour
{
    [SerializeField] int damagePerTick = 10;
    [SerializeField] float tickInterval = 0.25f;
    [SerializeField] DamageInstance.DamageType damageType = DamageInstance.DamageType.Spiritual;
    BoxCollider2D _box;
    SpriteRenderer[] _renderers;
    float _tickAcc;
    bool _armed;
    void Awake()
    {
        _box = GetComponent<BoxCollider2D>();
        _box.isTrigger = true;
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public void Configure(Vector2 sizeWorld, int dmgPerTick, float tickSeconds, float lifetimeSeconds)
    {
        damagePerTick = dmgPerTick;
        tickInterval = Mathf.Max(0.05f, tickSeconds);
        _box.size = Vector2.one;
        _box.offset = Vector2.zero;
        Destroy(gameObject, lifetimeSeconds);
    }
    public void SetArmed(bool armed)
    {
        _armed = armed;
        _box.enabled = armed;
    }
    public void SetVisualAlpha(float a)
    {
        foreach (var sr in _renderers)
        {
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (!_armed || !other.CompareTag("Player") || PlayerController.instance == null)
            return;
        _tickAcc += Time.deltaTime;
        while (_tickAcc >= tickInterval)
        {
            _tickAcc -= tickInterval;
            PlayerController.instance.TakeDamage(
                damagePerTick,
                DamageInstance.DamageSource.Enemy,
                damageType);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _tickAcc = 0f;
    }
}