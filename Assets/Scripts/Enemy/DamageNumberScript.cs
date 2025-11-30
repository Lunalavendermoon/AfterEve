using UnityEngine;

public class DamageNumberScript : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 1f);
        transform.position += offset;
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.5f), 0);

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
