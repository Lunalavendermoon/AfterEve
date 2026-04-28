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

    public static int CalculateTarotDrop(float luck, bool elite)
    {
        float tarotDropRate = elite ? 0.05f : 0.4f;
        float random = Random.Range(0f, 1f);
        if (random <= Mathf.Clamp(tarotDropRate * luck, 0f, 1f))
        {
            return 1; // Drop 1 tarot card
        }
        return 0; // No tarot card dropped
    }
}
