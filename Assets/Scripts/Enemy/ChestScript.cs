using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    private Dictionary<GameObject, int> chestContents = new Dictionary<GameObject, int>();

    void addItem(GameObject item, int amt)
    {
        if (chestContents.ContainsKey(item))
        {
            chestContents[item] += amt;
        }
        else
        {
            chestContents[item] = amt;
        }
    }

    void removeItem(GameObject item)
    {
        if (chestContents.ContainsKey(item))
        {
            chestContents.Remove(item);
        }
    }

    void emptyContents()
    {
        //TODO: Implement this method to empty the chest contents
    }
}
