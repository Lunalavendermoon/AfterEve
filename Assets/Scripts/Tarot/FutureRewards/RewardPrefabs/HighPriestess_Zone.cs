using System.Collections.Generic;
using UnityEngine;

public class HighPriestess_Zone : MonoBehaviour
{
    Enlightened_Effect playerEffect = new(-1f);

    Cursed_Effect enemyEffect = new(-1f, 0.4f);

    Dictionary<Collider2D, EffectInstance> effects = new();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerOrEnemy(other))
        {
            EffectInstance ei = other.gameObject.GetComponent<EffectManager>().AddEffect(GetCorrespondingEffect(other));
            if (!effects.TryAdd(other, ei))
            {
                effects[other] = ei;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerOrEnemy(other))
        {
            other.gameObject.GetComponent<EffectManager>().RemoveEffect(
                effects[other]
            );
        }
    }

    bool IsPlayerOrEnemy(Collider2D other)
    {
        return other.CompareTag("Player") || other.CompareTag("Enemy");
    }

    Effects GetCorrespondingEffect(Collider2D other)
    {
        return other.CompareTag("Player") ? playerEffect : enemyEffect;
    }
}