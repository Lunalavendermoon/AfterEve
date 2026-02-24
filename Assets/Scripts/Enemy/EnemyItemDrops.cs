using UnityEngine;

public class EnemyItemDrops : MonoBehaviour
{
    public static int CalculateShardDrop(float luck, bool elite)
    {
        float coinDropRate = elite ? 5f : 2f;

        float random = Random.Range(0f, coinDropRate);
        if (elite) random += 5f;

        int numShards = (int)(random * luck + 1);

        if (PlayerController.instance.playerAttributes.magicianPastBonusCoin)
        {
            Debug.Log("Added bonus shard from Magician Past tarot effect");
            numShards += Magician_Past.bonusCoinAmount;
        }

        return Mathf.Max(0, numShards);
    }
}
