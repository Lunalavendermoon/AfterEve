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

        int damageAfterReduction = Mathf.CeilToInt(amount * (1 - (enemyAttributes.basicDefense / (enemyAttributes.basicDefense + 100))));
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
        if (hitFlashCoroutine != null) StopCoroutine(hitFlashCoroutine);
        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    protected virtual IEnumerator HitFlash()
    {
        // Spine SkeletonAnimation
        var skeletonAnim = GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnim != null)
        {
            var mr = skeletonAnim.GetComponent<MeshRenderer>();
            if (hitFlashMaterial != null && mr != null)
            {
                Material originalMat = mr.material;
                mr.material = hitFlashMaterial;
                yield return new WaitForSeconds(hitFlashDuration);
                mr.material = originalMat;
            }
            else
            {
                var skeleton = skeletonAnim.skeleton;
                Color originalColor = new Color(skeleton.R, skeleton.G, skeleton.B, skeleton.A);
                skeleton.SetColor(new Color(1f, 0.3f, 0.3f, 1f));
                yield return new WaitForSeconds(hitFlashDuration);
                skeleton.SetColor(originalColor);
            }
            yield break;
        }

        // SpriteRenderer fallback
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length > 0)
        {
            Color[] originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) originalColors[i] = renderers[i].color;
            foreach (var r in renderers) r.color = Color.white;
            yield return new WaitForSeconds(hitFlashDuration);
            for (int i = 0; i < renderers.Length; i++) renderers[i].color = originalColors[i];
        }
    }

    protected void ShowFloatingText(int damageAfterReduction, DamageInstance.DamageType damageType)
    {
        Debug.Log($"### Floating text {damageAfterReduction} {damageType}");
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
                Debug.Log($"### Floating text, is text null: {textColor == null}");
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
