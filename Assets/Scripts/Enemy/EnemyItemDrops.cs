using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class EnemyItemDrops : MonoBehaviour
{
    public static void ItemDrop(float luck, bool elite)
    {
        float[] dropRates = { 0.05f, 0.1f, 2 };
        if (elite) dropRates = new float[] { 0.4f, 0.6f, 5 };

        float random = Random.Range(0, 1);
        if (random < dropRates[0] * luck)
        {
            Debug.Log("Dropped Tarot Card");
        }

        random = Random.Range(0, 1);
        if (random < dropRates[1] * luck)
        {
            Debug.Log("Dropped Potion");
        }

        random = Random.Range(0, dropRates[2]);
        if (elite) random += 5;
        int numShards = (int)(random * luck + 1);
        if(numShards > 0)
        {
            Debug.Log("Dropped " + numShards + " Shards");
        }
    }

    public static void ItemDrop(float luck)
    {
        ItemDrop(luck, false);
    }
}
