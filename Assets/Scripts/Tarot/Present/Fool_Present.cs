using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Fool_Present : Present_TarotCard
{
    EnemyBase lastEnemy;

    public Fool_Present(string s, int q) : base(s, q)
    {
        lastEnemy = null;
        effects.Add(new FireRate_Effect(-1, 1.2f));
    }

    void OnEnable()
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    void OnDisable()
    {
        Projectile.OnEnemyHit -= HandleEnemyHit;
    }
    
    private void HandleEnemyHit(EnemyBase enemy)
    {
        if (lastEnemy != enemy)
        {
            PlayerController.instance.gameObject.GetComponent<EffectManager>().AddEffect(new Strength_Effect(0.1f, 1.2f));
        }
        lastEnemy = enemy;
    }
}
