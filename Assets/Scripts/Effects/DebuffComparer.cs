using System.Collections.Generic;

public class DebuffComparer : IComparer<EffectInstance>
{
    public int Compare(EffectInstance x, EffectInstance y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        int cmp = x.effect.effectRate.CompareTo(y.effect.effectRate);
        
        // if we return 0, the comparator will consider two Effects w/ the same effectRate identical even if they're separate instances
        // this causes problems when we try to add two Effects with the same effectRate to the same set
        // i.e. the second Effect won't be added to the set at all

        // therefore: if the two Effects are separate instances, tiebreak arbitrarily to make sure they're not equal

        return cmp == 0 ? x.effectId.CompareTo(y.effectId) : cmp;
    }
}