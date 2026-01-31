using UnityEngine;

[System.Serializable]
public class Chest_base : InteractableEntity
{
    public int shards = 0;
    public int potions = 0;
    public int tarotCards = 0;

    public void AddShards(int amount)
    {
        shards += Mathf.Max(0, amount);
    }

    public void AddPotion(int amount = 1)
    {
        potions += Mathf.Max(0, amount);
    }

    public void AddTarotCard(int amount = 1)
    {
        tarotCards += Mathf.Max(0, amount);
    }


    public void RemoveShards(int amount)
    {
        shards = Mathf.Max(0, shards - amount);
    }

    public void RemovePotion(int amount = 1)
    {
        potions = Mathf.Max(0, potions - amount);
    }

    public void RemoveTarotCard(int amount = 1)
    {
        tarotCards = Mathf.Max(0, tarotCards - amount);
    }

    public override void TriggerInteraction()
    {
        // TODO player actually obtains items
        Debug.Log($"### OPEN CHEST: {shards} shards, {potions} potions, {tarotCards} cards");

        GameManager.instance.ClearCombatRoom();

        Destroy(gameObject);
    }
}
