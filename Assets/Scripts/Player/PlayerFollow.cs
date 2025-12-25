using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
