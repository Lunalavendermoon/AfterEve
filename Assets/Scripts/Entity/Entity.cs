using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Attributes (ScriptableObject)")]
    [SerializeField] private EntityAttributes attributes;

    [Header("Runtime State")]
    [SerializeField] private int currentHP;
    [SerializeField] private bool isDead;

    // Optional hooks for UI / SFX / VFX / analytics
    public event Action<int, int> OnHealthChanged; // (currentHP, maxHP)
    public event Action<int> OnDamaged;            // (damageApplied)
    public event Action OnDied;

    private Coroutine freezeRoutine;

    public EntityAttributes Attributes => attributes;

    public int MaxHP => attributes != null ? attributes.maxHitPoints : 0;
    public int CurrentHP => currentHP;
    public bool IsDead => isDead;

    private void Awake()
    {
        // If you prefer to initialize elsewhere, you can remove this.
        InitializeFromAttributes();
    }

    /// <summary>
    /// Call this if you assign Attributes at runtime or want to reset HP.
    /// </summary>
    public void InitializeFromAttributes(bool resetHPToMax = true)
    {
        if (attributes == null)
        {
            Debug.LogWarning($"{name}: EntityAttributes not assigned.", this);
            currentHP = 0;
            isDead = false;
            return;
        }

        if (resetHPToMax)
        {
            currentHP = Mathf.Max(1, attributes.maxHitPoints);
        }
        else
        {
            currentHP = Mathf.Clamp(currentHP, 0, attributes.maxHitPoints);
        }

        isDead = (currentHP <= 0);
        OnHealthChanged?.Invoke(currentHP, attributes.maxHitPoints);
    }

    // commented this out since it seems to be unused
    /// <summary>
    /// Applies incoming damage AFTER external damage calculation.
    /// (Your ObstacleData / EnemyAttributes etc should compute the damage amount.)
    /// This method handles runtime defenses like player shield.
    /// </summary>
    // public void TakeDamage(int amount)
    // {
    //     if (isDead || attributes == null) return;

    //     int damageApplied = Mathf.Max(0, amount);

    //     // Player-specific shield handling (simple + robust)
    //     if (attributes is PlayerAttributes playerAttr)
    //     {
    //         // If shield exists, reduce damage by shield value
    //         damageApplied = Mathf.Max(0, damageApplied - playerAttr.shield);

    //         // Optional: shield depletes by hit count (only if you want that mechanic)
    //         if (playerAttr.hitCountShield > 0)
    //         {
    //             playerAttr.hitCountShield -= 1;
    //             if (playerAttr.hitCountShield <= 0)
    //             {
    //                 // Shield "breaks" after N hits
    //                 playerAttr.shield = 0;
    //             }
    //         }
    //     }

    //     if (damageApplied <= 0)
    //         return;

    //     currentHP = Mathf.Max(0, currentHP - damageApplied);
    //     OnDamaged?.Invoke(damageApplied);
    //     OnHealthChanged?.Invoke(currentHP, attributes.maxHitPoints);

    //     if (currentHP <= 0)
    //     {
    //         Die();
    //     }
    // }

    public void Heal(int amount)
    {
        if (isDead || attributes == null) return;

        int heal = Mathf.Max(0, amount);
        if (heal == 0) return;

        currentHP = Mathf.Min(attributes.maxHitPoints, currentHP + heal);
        OnHealthChanged?.Invoke(currentHP, attributes.maxHitPoints);
    }

    /// <summary>
    /// Applies "freeze" as paralysis for duration.
    /// Uses EntityAttributes.isParalyzed flag.
    /// </summary>
    public void ApplyFreeze(float duration)
    {
        if (isDead || attributes == null) return;

        float d = Mathf.Max(0f, duration);
        if (d <= 0f) return;

        if (freezeRoutine != null)
        {
            StopCoroutine(freezeRoutine);
        }
        freezeRoutine = StartCoroutine(FreezeCoroutine(d));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        attributes.isParalyzed = true;

        float t = 0f;
        while (t < duration)
        {
            // If you want freeze to be cancelled by something, add checks here.
            t += Time.deltaTime;
            yield return null;
        }

        attributes.isParalyzed = false;
        freezeRoutine = null;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHP = 0;

        OnDied?.Invoke();

        // Minimal default behavior:
        // You can replace with pooling, ragdoll, animation triggers, etc.
        Destroy(gameObject);
    }

    // Optional helper for obstacles / enemies that want a quick type check
    public bool IsPlayer()
    {
        return attributes is PlayerAttributes;
    }

    public bool IsEnemy()
    {
        return attributes is EnemyAttributes;
    }
}
