using UnityEngine;

[System.Serializable]
public class Chest_base : InteractableEntity
{
    public int coins = 0;
    public int TarotCardCount = 0;  

    public void AddTarotCards(int count)
    {
        TarotCardCount += Mathf.Max(0, count);
    }

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
    }

    public void RemoveCoins(int amount)
    {
        coins = Mathf.Max(0, coins - amount);
    }

    public override void TriggerInteraction()
    {
        Vector3 chestPos = transform.position;
        PlayerController.instance.ChangeCoins(coins);

        if (ChestRewardManager.instance != null)
        {
            ChestRewardManager.instance.ShowChestRewardMenu(true, TarotCardCount);
            ChestRewardManager.instance.SetLastChestWorldPos(chestPos);
        }

        Destroy(gameObject);
    }

    protected override string GetInteractionType()
    {
        return "Chest";
    }
}
