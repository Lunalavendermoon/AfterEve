using Pathfinding;
using System;
using UnityEngine;

public class ReboundProjectile : MonoBehaviour
{

    private Vector2 direction;
    private Rigidbody2D rb;
    private float commitmentRadus = 1f;
    private float overshootDistance = 0.5f;
    private int maxRebounds = 5;
    private int currentRebounds = 0;
    private Transform tempTarget;
    public AIPath agent;
    public AIDestinationSetter destinationSetter;
    PlayerController player;

    public enum projectileState
    {
        Tracking,
        Committed,
        Recalibrating

    }

    private projectileState currentState;

    /* In this script a homing projectiles is fired at the player its passes through the player  
     * then decelartes till it reaches a point behind the player before repeating the process
     * it repeats this process for a set number of times before it is destroyed
     * approach: set destination to a point behind player position till the player is in committment range
     * the head to point behind the player and restart the process
     */
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.instance;
        direction = (player.transform.position - transform.position).normalized;
        agent=GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        tempTarget = new GameObject("TempTarget").transform;
        currentState = projectileState.Tracking;
    }

    private void Update()
    {
        if (currentState == projectileState.Tracking)
        {
            
            if (Vector2.Distance(transform.position, player.transform.position) <= commitmentRadus)
                {
                    currentState = projectileState.Committed;
                // Set the target to a point behind the player
                    tempTarget.position = (Vector2)player.transform.position - direction * overshootDistance;
                destinationSetter.target = tempTarget;
            }
        }
        else if (currentState == projectileState.Committed)
        {
            float distanceToTarget = Vector2.Distance(transform.position, tempTarget.position);
            if ((distanceToTarget < 0.1f))
            {
                currentRebounds++;
                if (currentRebounds >= maxRebounds)
                {
                    Destroy(gameObject);
                }
                else
                {
                    currentState = projectileState.Recalibrating;
                }
            }
        }
        else if (currentState == projectileState.Recalibrating)
        {
            destinationSetter.target = player.transform;
            currentState = projectileState.Tracking;

        }
    }


}

