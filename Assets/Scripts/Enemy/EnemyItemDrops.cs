using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class EnemyItemDrops : MonoBehaviour
{
    public static void ItemDrop(float luck, bool elite, Chest_base chest)
    {
        float coinDropRate = 2;
        if (elite) coinDropRate = 5;

        float random = Random.Range(0, coinDropRate);
        if (elite) random += 5;
        int numShards = (int)(random * luck + 1);
        if (PlayerController.instance.playerAttributes.magicianPastBonusCoin)
        {
            Debug.Log("Added bonus shard from Magician Past tarot effect");
            numShards += Magician_Past.bonusCoinAmount;
        }
        if(numShards > 0)
        {
            chest.AddCoins(numShards);
            Debug.Log("Dropped " + numShards + " Shards");
        }
    }
}
