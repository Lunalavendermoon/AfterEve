using UnityEngine;

public class Socket : MonoBehaviour
{
    public enum Kind { Door, Wall }
    public enum Dir { N, E, S, W }
    public Kind kind = Kind.Door;
    public Dir dir = Dir.N;
    public float width = 4f; // ÃÅ¾»¿í£¬ÓÃÓÚÆ¥Åä
}
