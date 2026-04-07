using UnityEngine;

public class SlowEffectColliderScript : MonoBehaviour
{
    [Tooltip("Speed multiplier while inside (e.g. 0.75 = move at 75% speed).")]
    [Range(0.05f, 1f)]
    [SerializeField] float speedMultiplier = 0.75f;
    [Tooltip("Duration passed to Slow_Effect; refreshed while the player stays in the trigger.")]
    [SerializeField] float slowDurationSeconds = 2f;
    [Tooltip("How often to remove/reapply slow to reset incremental stacking (Slow ticks every 1s).")]
    [SerializeField] float refreshIntervalSeconds = 0.85f;
    Collider2D _col;
    PlayerEffectManager _playerFx;
    EffectInstance _slowInstance;
    float _refreshCountdown;
    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
    }
    void OnDisable()
    {
        ClearSlow();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var pem = other.GetComponentInParent<PlayerEffectManager>();
        if (pem == null) return;
        _playerFx = pem;
        _refreshCountdown = 0f;
        ApplySlow();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (_playerFx == null) return;
        if (other.GetComponentInParent<PlayerEffectManager>() != _playerFx) return;
        _refreshCountdown -= Time.deltaTime;
        if (_refreshCountdown <= 0f)
        {
            RefreshSlow();
            _refreshCountdown = refreshIntervalSeconds;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        var pem = other.GetComponentInParent<PlayerEffectManager>();
        if (pem != _playerFx) return;
        ClearSlow();
        _playerFx = null;
    }
    void ApplySlow()
    {
        if (_playerFx == null) return;
        var attrs = PlayerController.instance != null
            ? PlayerController.instance.playerAttributes
            : null;
        if (attrs == null) return;
        _slowInstance = _playerFx.AddEffect(
            new Slow_Effect(slowDurationSeconds, speedMultiplier),
            attrs);
    }
    void RefreshSlow()
    {
        if (_playerFx == null) return;
        if (_slowInstance != null)
            _playerFx.RemoveEffect(_slowInstance);
        ApplySlow();
    }
    void ClearSlow()
    {
        if (_playerFx != null && _slowInstance != null)
            _playerFx.RemoveEffect(_slowInstance);
        _slowInstance = null;
    }
}
