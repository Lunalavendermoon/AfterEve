public class InteractableShop : InteractableEntity
{
    public override void TriggerInteraction()
    {
        ShopManager.instance.ShowShop(true);
    }

    protected override string GetInteractionType()
    {
        return "Shop";
    }
}