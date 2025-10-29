public class EnemyEffectManager : EffectManager
{
    public EnemyBase enemy;

    // constant, storing the base attribute values
    public EnemyAttributes baseEnemyAttributes;

    // changes during runtime based on the effects the enemy currently has
    public EnemyAttributes effectEnemyAttributes;

    public override void ApplyEffects()
    {
        effectEnemyAttributes = Instantiate(baseEnemyAttributes);
        ApplyEffectsHelper((effect) => { effect.ApplyEnemyEffect(effectEnemyAttributes, enemy); });
        enemy.enemyAttributes = effectEnemyAttributes;
    }
}