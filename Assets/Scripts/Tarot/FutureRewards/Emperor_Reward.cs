using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class Emperor_Reward : Future_Reward
{
    public const float paralyzeDuration = 5f;

    public Emperor_Reward() : base(Emperor_Future.uses, Emperor_Future.cd, TarotCard.Arcana.Emperor)
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

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(paralyzeDuration),
            Mathf.RoundToInt(displayCooldown), displayUses };
    }
}