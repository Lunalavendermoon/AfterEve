using Pathfinding;
using Spine.Unity;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public abstract class EnemyBase : MonoBehaviour
{
    public EnemyAttributes baseEnemyAttributes;

    public EnemyAttributes enemyAttributes;

    public EnemyEffectManager enemyEffectManager;
    public EnemySpawnerScript spawner;
    protected Transform tempTarget;
    // Enemy attributes
    public int health;
    public float speed;


    //Pathfinding agent
    public AIPath agent;
    public AIDestinationSetter destinationSetter;



    public GameObject floatingTextPrefab;
    public Rigidbody2D rb;

    [Header("Hit Flash")]
    [SerializeField] protected float hitFlashDuration = 0.08f;
    [Tooltip("Hit Effect Material")]
    [SerializeField] protected Material hitFlashMaterial;

    private Coroutine hitFlashCoroutine;

    private Material cachedSpineOriginalMaterial;
    private Color cachedSpineOriginalColor = Color.white;
    private bool spineBaselineCached;
    private float hitFlashRestoreAt;
    private bool hitFlashApplied;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDamageTaken;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDeath;

    //lovers clone
    protected float marked = 1;

    //chaining
    protected bool isChained;
    protected float chainTime;


    public bool IsChained()
    {
        return isChained;
    }

    public void Chain(float f)
    {
        isChained = true;
        chainTime = Time.time + f;
    }

    public void Mark(float f)
    {
        marked = f;
    }

    public virtual void Die()
    {

    }





    public virtual void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        amount = (int)(amount * marked);
        if (enemyAttributes == null) return;
        int damageAfterReduction = enemyAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(dmgType));
        health -= damageAfterReduction;

        OnEnemyDamageTaken?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyTakeDamage, this.transform.position);

        TriggerHitFlash();

        // Damage numbers
        ShowFloatingText(damageAfterReduction, dmgType);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            OnEnemyDeath?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);
            Die();
        }
    }

    // Can make this virtual and override for bosses w/ multiple phases if needed.
    public bool IsAlive()
    {
        return health > 0;
    }

    public virtual void Heal(int amount)
    {
        health = Math.Clamp(health + amount, 0, enemyAttributes.maxHitPoints);
        Debug.Log($"{gameObject.name} healed {amount}, current health: {health}");
    }

    public virtual void Pathfinding(Transform target)
    {
        destinationSetter.target = target;
    }



    protected void TriggerHitFlash()
    {
        hitFlashRestoreAt = Time.time + hitFlashDuration;
        if (hitFlashCoroutine == null)
            hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private void CacheSpineBaselineIfNeeded(SkeletonAnimation skeletonAnim)
    {
        if (spineBaselineCached) return;
        if (skeletonAnim == null) return;

        var mr = skeletonAnim.GetComponent<MeshRenderer>();
        if (mr != null) cachedSpineOriginalMaterial = mr.sharedMaterial;

        var skeleton = skeletonAnim.skeleton;
        if (skeleton != null)
            cachedSpineOriginalColor = new Color(skeleton.R, skeleton.G, skeleton.B, skeleton.A);

        spineBaselineCached = true;
    }

    protected virtual IEnumerator HitFlash()
    {
        hitFlashApplied = false;

        // Spine SkeletonAnimation
        var skeletonAnim = GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnim != null)
        {
            CacheSpineBaselineIfNeeded(skeletonAnim);
            var mr = skeletonAnim.GetComponent<MeshRenderer>();
            if (hitFlashMaterial != null && mr != null)
            {
                var skeleton = skeletonAnim.skeleton;

                mr.sharedMaterial = hitFlashMaterial;
                if (skeleton != null) skeleton.SetColor(Color.white);
                hitFlashApplied = true;

                while (Time.time < hitFlashRestoreAt)
                    yield return null;

                mr.sharedMaterial = cachedSpineOriginalMaterial;
                if (skeleton != null) skeleton.SetColor(cachedSpineOriginalColor);
            }
            else
            {
                var skeleton = skeletonAnim.skeleton;
                skeleton.SetColor(new Color(1f, 0.3f, 0.3f, 1f));
                hitFlashApplied = true;

                while (Time.time < hitFlashRestoreAt)
                    yield return null;

                if (skeleton != null) skeleton.SetColor(cachedSpineOriginalColor);
            }

            hitFlashCoroutine = null;
            yield break;
        }

        // SpriteRenderer fallback
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length > 0)
        {
            Color[] originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) originalColors[i] = renderers[i].color;
            foreach (var r in renderers) r.color = new Color(1f, 0.3f, 0.3f, 1f);
            while (Time.time < hitFlashRestoreAt)
                yield return null;
            for (int i = 0; i < renderers.Length; i++) renderers[i].color = originalColors[i];
        }

        hitFlashCoroutine = null;
    }

    protected void ShowFloatingText(int damageAfterReduction, DamageInstance.DamageType damageType)
    {
        if (floatingTextPrefab != null)
        {
            //Debug.Log("Showing floating text for damage: " + damageAfterReduction);
            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            var tmp = floatingText.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAfterReduction.ToString();

                // Set text color: red for physical dmg, blue for spiritual
                var textColor = floatingText.GetComponentInChildren<TMP_Text>();
                if (textColor != null)
                {
                    textColor.color = damageType switch
                    {
                        DamageInstance.DamageType.Physical => Color.red,
                        DamageInstance.DamageType.Spiritual => Color.blue,
                        _ => Color.red
                    };
                }
            }

        }
    }
    public virtual void Pathfinding(Vector3 targetPosition)
    {
        if (destinationSetter == null) return;

        // Create a temporary GameObject internally
        if (tempTarget == null)
        {
            GameObject go = new GameObject($"{gameObject.name}_TempTarget");
            tempTarget = go.transform;
        }

        tempTarget.position = targetPosition;
        destinationSetter.target = tempTarget;
    }

    private void OnDestroy()
    {

    }

    public virtual bool ShouldBlockEffect(Effects effect)
    {
        return false;
    }
}
