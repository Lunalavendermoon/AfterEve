using System;
using System.Collections.Generic;
using UnityEngine;


// Manages the shields present on the player
// No enemy equivalent (IDT any enemies use shields besides false human boss) but can make one if needed!
public class ShieldManager : MonoBehaviour
{
    protected Shield miscShield = new Regular_Shield(0, -1);

    // only store shields that can time out
    protected List<ShieldInstance> shieldTimers = new();

    // only store shields that never time out
    protected List<ShieldInstance> permanentShields = new();

    // stores every single shield that's currently active
    public Dictionary<Shield.ShieldType, List<ShieldInstance>> allShields = new();

    long shieldCount = 0;

    public virtual ShieldInstance AddShield(Shield shield)
    {
        ShieldInstance si = new ShieldInstance(shield, shieldCount++);

        if (allShields.ContainsKey(shield.shieldType))
        {
            allShields[shield.shieldType].Add(si);
        }
        else
        {
            allShields[shield.shieldType] = new() { si };
        }

        if (shield.isPermanent)
        {
            permanentShields.Add(si);
        }
        else
        {
            shieldTimers.Add(si);
        }
        return si;
    }

    public virtual void RemoveShield(ShieldInstance si)
    {
        if (si == null)
        {
            return;
        }

        if (si.shield.isPermanent)
        {
            permanentShields.Remove(si);
        }
        else
        {
            shieldTimers.Remove(si);
        }

        allShields[si.shield.shieldType].Remove(si);
    }

    void Update()
    {
        float time_elapsed = Time.deltaTime;

        for (int i = shieldTimers.Count - 1; i >= 0; i--)
        {
            ShieldInstance ei = shieldTimers[i];
            ei.SubtractTime(time_elapsed);

            if (ei.IsExpired())
            {
                RemoveShield(ei);
            }
        }
    }

    // subtracts damage amount from shields, and returns the remaining damage after being partially absorbed by shields
    public int TakeShieldDamage(int amount)
    {
        Shield.ShieldType[] types = (Shield.ShieldType[])Enum.GetValues(typeof(Shield.ShieldType));

        foreach (Shield.ShieldType type in types)
        {
            if (allShields.ContainsKey(type))
            {
                for (int i = allShields[type].Count - 1; i >= 0; --i)
                {
                    Shield shield = allShields[type][i].shield;
                    amount = shield.TakeShieldDamage(amount);

                    if (shield.IsDepleted())
                    {
                        allShields[type].RemoveAt(i);
                    }

                    if (amount == 0)
                    {
                        Debug.Log($"damage {type}");
                        return amount;
                    }
                }
            }
            if (type == Shield.ShieldType.Regular)
            {
                amount = miscShield.TakeShieldDamage(amount);
                if (amount == 0)
                {
                    return amount;
                }
            }
        }
        return amount;
    }

    public void GainRegularShield(int amount)
    {
        miscShield.GainShield(amount);
    }

    public int GetTotalShield(Shield.ShieldType type)
    {
        int count = 0;
        if (type == Shield.ShieldType.Regular)
        {
            count += miscShield.shieldAmount;
        }

        if (!allShields.ContainsKey(type))
        {
            return count;
        }

        foreach (ShieldInstance si in allShields[type])
        {
            count += si.shield.shieldAmount;
        }
        return count;
    }
}