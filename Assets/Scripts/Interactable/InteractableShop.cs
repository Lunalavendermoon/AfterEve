public class InteractableShop : InteractableEntity
{
    protected bool firstInteraction = true;

    public override void TriggerInteraction()
    {
        ShopManager.instance.ShowShop(true, firstInteraction);
        firstInteraction = false;
    }
}