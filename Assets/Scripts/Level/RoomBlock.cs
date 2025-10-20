using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoomBlock : MonoBehaviour
{
    public string blockId = "Cross_16";
    public string type = "junction";      // "junction"/"arena"/...
    public Vector2Int size = new(16, 16);  // XZ Õ¼µØ

    [HideInInspector] public Socket[] sockets;
    [HideInInspector] public Collider bounds;

    void Awake() { Cache(); }
    void OnValidate() { Cache(); }

    void Cache()
    {
        sockets = GetComponentsInChildren<Socket>(true);
        bounds = GetComponent<Collider>();
    }
}
