using System.Collections.Generic;
using UnityEngine;

public class Emperor_Reward : Future_Reward
{
    public const float paralyzeDuration = 5f;

    public Emperor_Reward(Future_TarotCard card) : base(Emperor_Future.uses, Emperor_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        List<EnemyBase> enemies = EnemySpawnerScript.instance.enemies;

        Debug.Log($"Triggered Emperor skill, {enemies.Count} enemies found");

        foreach (EnemyBase enemy in enemies)
        {
            if (enemy.enemyAttributes.enemyType != EnemyAttributes.EnemyType.Basic)
            {
                continue;
            }
            enemy.enemyEffectManager.AddEffect(new Paralyze_Effect(paralyzeDuration), enemy.enemyAttributes);
        }
    }

    public override string GetName()
    {
        return "Emperor Skill";
    }
}