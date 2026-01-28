public class ShieldInstance
{
    public Shield shield;

    public float timer;

    // used to compare unique debuff instances in the case that their effectRates are equal
    public long effectId;

    public ShieldInstance(Shield s, long effectId)
    {
        shield = s;
        timer = s.shieldDuration;

        this.effectId = effectId;
    }

    public void SubtractTime(float delta_t)
    {
        timer -= delta_t;
    }

    public bool IsExpired()
    {
        return shield.shieldAmount <= 0 || (!shield.isPermanent && timer <= 0f);
    }
}