using System.Collections.Generic;

public class DebuffComparer : IComparer<EffectScriptableObject>
{
    public int Compare(EffectScriptableObject x, EffectScriptableObject y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        return x.effectRate.CompareTo(y.effectRate);
    }
}