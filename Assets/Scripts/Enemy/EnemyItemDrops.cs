using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class EnemyItemDrops : MonoBehaviour
{
    public static void ItemDrop(float luck, bool elite, Chest_base chest)
    {
        float[] dropRates = { 0.05f, 0.1f, 2 };
        if (elite) dropRates = new float[] { 0.4f, 0.6f, 5 };

        float random = Random.Range(0, 1);
        if (random < dropRates[0] * luck)
        {
            chest.AddTarotCard(1);
            Debug.Log("Dropped Tarot Card");
        }

        random = Random.Range(0, 1);
        if (random < dropRates[1] * luck)
        {
            chest.AddPotion(1);
            Debug.Log("Dropped Potion");
        }

        random = Random.Range(0, dropRates[2]);
        if (elite) random += 5;
        int numShards = (int)(random * luck + 1);
        if(numShards > 0)
        {
            chest.AddShards(numShards);
            Debug.Log("Dropped " + numShards + " Shards");
        }
    }

    public static void ItemDrop(float luck)
    {
        ItemDrop(luck, false, null);
    }
}
