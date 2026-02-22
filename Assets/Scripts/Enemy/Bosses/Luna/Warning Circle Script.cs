using UnityEngine;

public class WarningCircleScript : MonoBehaviour
{
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer+= Time.deltaTime;
        if (timer > 1f)
            Destroy(gameObject);
    }
}
