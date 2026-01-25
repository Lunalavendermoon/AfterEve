using Pathfinding;
using UnityEngine;

// TODO: add attack mirroring
public class Lovers_Summon : MonoBehaviour
{
    // pathfinding
    public AIPath agent;
    public AIDestinationSetter destinationSetter;

    void Awake()
    {
        agent = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.target = PlayerController.instance.transform;

        // TODO modify max speed dynamically as player's speed stat changes?
        agent.maxSpeed = IPlayerState.speedCoefficient * PlayerController.instance.playerAttributes.speed;
    }
}