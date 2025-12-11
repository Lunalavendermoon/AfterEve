using UnityEngine;

public class Regeneration_Effect : Multiplier_Effects
{
    /// <summary>
    /// HP regenerate by a percentage per second
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplyAdditive"></param>
    public Regeneration_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.HP;
        isDebuff = false;
        isIncremental = true;
        incrementInterval = 1;

        effectApplication = Application.Multiplier;

        iconType = IconType.BuffRegen;
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        if (increment)
        {
            int regenAmount = (int)(playerAttributes.maxHitPoints * effectRate);
            PlayerController.instance.Heal(regenAmount);

            Debug.Log("Regenerated amount: " + regenAmount);
            initialApplication = false;
        }
    }

    public override void ApplyEnemyEffect(EnemyAttributes enemyAttributes, EnemyBase enemy, bool increment)
    {
        if (increment)
        {
            int regenAmount = (int)(enemyAttributes.maxHitPoints * effectRate);
            enemy.Heal(regenAmount);

            Debug.Log("Regenerated amount: " + regenAmount);
            initialApplication = false;
        }
    }
}
