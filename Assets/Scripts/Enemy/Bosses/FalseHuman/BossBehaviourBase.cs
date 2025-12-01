using UnityEngine;

public abstract class BossBehaviourBase : MonoBehaviour
{
    // 2 states : in cooldown and attack 
    // during in cooldown will use the movement func
    // after cooldown will roll a dice to see which of the 5 attacks 
    // probabilities for each attack will be defined in the particular boss script
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Movement(int time);
    public abstract void Attack1(int time);
    public abstract void Attack2(int time);
    public abstract void Attack3(int time);
    public abstract void Attack4(int time);

    public abstract void Attack5(int time);




}
