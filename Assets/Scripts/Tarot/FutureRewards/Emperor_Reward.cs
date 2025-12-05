using System.Collections.Generic;
using UnityEngine;

public class Emperor_Reward : Future_Reward
{
    const float paralyzeDuration = 5f;

    public Emperor_Reward(Future_TarotCard card) : base(7, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO: filter out bosses and elites -- this skill should only paralyze basic mobs
        Debug.Log("Triggered Emperor skill");

        List<EnemyBase> enemies = EnemySpawnerScript.instance.enemies;
        foreach (EnemyBase enemy in enemies)
        {
            enemy.enemyEffectManager.AddEffect(new Paralyze_Effect(paralyzeDuration));
        }
    }

    public override string GetName()
    {
        return "Emperor Skill";
    }
}