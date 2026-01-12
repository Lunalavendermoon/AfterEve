using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Fool_Present : Present_TarotCard
{
    float[] fireRateIncrease = {1.2f, 1.3f, 1.4f, 1.5f, 1.6f};
    float[] additionalDmg = {1.2f, 1.25f, 1.3f, 1.35f, 1.4f};

    EnemyBase lastEnemy;

    public Fool_Present(int q) : base(q)
    {
        cardName = "Fool_Present";
        lastEnemy = null;

        effects.Add(new FireRate_Effect(-1, fireRateIncrease[level]));
    }

    protected override void ApplyListeners()
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    protected override void RemoveListeners()
    {
        Projectile.OnEnemyHit -= HandleEnemyHit;
    }
    
    private void HandleEnemyHit(EnemyBase enemy)
    {
        if (lastEnemy != enemy)
        {
            PlayerController.instance.gameObject.GetComponent<EffectManager>().AddEffect(new Strength_Effect(0.1f, additionalDmg[level]));
        }
        lastEnemy = enemy;
    }
}
