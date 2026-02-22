using UnityEngine;

public class Hermit_Present : Present_TarotCard
{
    public Hermit_Present(int q) : base(q)
    {
        cardName = "Hermit_Present";
        arcana = Arcana.Hermit;
    }

    protected override void GetLocalizedDesc()
    {
        // TODO finish this
        base.GetLocalizedDesc();
        desc.TableEntryReference = "FoolPresent";
        desc.Arguments = new object[] { "temp", "temp" };
    }
}
