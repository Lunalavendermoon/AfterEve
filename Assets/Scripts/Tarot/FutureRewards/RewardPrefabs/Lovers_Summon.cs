using Pathfinding;
using UnityEngine;

// TODO: add attack mirroring
public class Lovers_Summon : MonoBehaviour
{
    public AIPath agent;
    public AIDestinationSetter destinationSetter;

    void Awake()
    {
        agent = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.target = PlayerController.instance.transform;
    }
}