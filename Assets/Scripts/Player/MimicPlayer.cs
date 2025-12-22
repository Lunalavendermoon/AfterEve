using UnityEngine;

public class MimicPlayer : MonoBehaviour
{

    void Update()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 2f)
            transform.position = Vector3.MoveTowards(transform.position,PlayerController.instance.transform.position, 2 * Time.deltaTime);
    }
}
