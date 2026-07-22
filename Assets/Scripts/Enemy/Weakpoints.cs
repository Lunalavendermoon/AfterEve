using UnityEngine;

public class Weakpoints : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateWeakPointPosition();
    }

    //

    public void updateWeakPointPosition()
    {
        //roate the gameobject randomly on the z axis
        transform.Rotate(0, 0, Random.Range(0, 360));
    }

    //every 5 seconds, rotate the gameobject randomly on the z axis
    void Update()
    {
        if (Time.frameCount % 300 == 0)
        {
            updateWeakPointPosition();
        }
    }
}
