using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Strength_Zone : MonoBehaviour
{
    const float timeBetweenDamage = 0.5f;
    const float weaponDamagePercent = 0.3f;
    float timer = timeBetweenDamage + 0.1f;

    Slow_Effect enemyEffect = new(-1f, 0.5f);

    Dictionary<Collider2D, EffectInstance> effects = new();

    bool playerInside = false;

    List<EnemyBase> enemiesInside = new();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EffectInstance ei = other.gameObject.GetComponent<EffectManager>().AddEffect(enemyEffect,
                other.gameObject.GetComponent<EnemyBase>().enemyAttributes);
            if (!effects.TryAdd(other, ei))
            {
                effects[other] = ei;
            }
            enemiesInside.Add(other.gameObject.GetComponent<EnemyBase>());
        }
        else if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EffectManager>().RemoveEffect(
                effects[other]
            );
            enemiesInside.Remove(other.gameObject.GetComponent<EnemyBase>());
        }
        else if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < timeBetweenDamage)
        {
            return;
        }
        timer = 0f;

        // Currently, damage snapshots player's current weapon damage but isn't affected by other buffs
        int rawDamage = 200 + (int)(weaponDamagePercent * PlayerController.instance.playerAttributes.damage);

        if (playerInside)
        {
            PlayerController.instance.TakeDamage(rawDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
        }
        // handles dealing dmg to enemies, takes into account enemies dying mid-loop & causing enemiesInside to shrink
        // TODO: might not handle enemies spawning mid-loop correctly
        // will need to change this code if any enemies spawn new enemies upon taking damage/dying
        int i = enemiesInside.Count - 1;
        while (i >= 0) {
            int prevLen = enemiesInside.Count;
            EnemyBase enemy = enemiesInside[i];
            enemy.TakeDamage(rawDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
            if (enemiesInside.Count != prevLen)
            {
                // decrement i according to length of the new list
                i = enemiesInside.Count - (prevLen - i);
            } else
            {
                // decrement i like a normal pointer
                --i;
            }
        }
    }
}