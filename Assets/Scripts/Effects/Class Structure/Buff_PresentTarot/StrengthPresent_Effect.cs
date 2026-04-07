using UnityEngine;

public class StrengthPresent_Effect : Effects
{
    int numPierces;
    float bulletPierceDmgDecrease;

    public StrengthPresent_Effect(float bulletPierceDmgDecrease) : base(-1)
    {
        numPierces = Mathf.FloorToInt(100f / bulletPierceDmgDecrease);
        this.bulletPierceDmgDecrease = bulletPierceDmgDecrease;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.bulletPierces = numPierces;
        playerAttributes.bulletPierceDmgDecrease = bulletPierceDmgDecrease;
    }
}