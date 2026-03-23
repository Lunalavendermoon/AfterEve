public class InteractableShop : InteractableEntity
{

    public override void TriggerInteraction()
    {
        ShopManager.instance.ShowShop(true);
    }
}