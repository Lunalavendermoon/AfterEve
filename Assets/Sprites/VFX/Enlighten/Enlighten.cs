using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Enlighten : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    private Material mat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int fps = 12;

    void Start()
    {
        mat = sr.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Blink());
        }
    }

    public IEnumerator Blink()
    {
        for (int i = 0; i <= 4; i++)
        {
            mat.SetFloat("_Index", i);
            yield return new WaitForSeconds(1f/fps);
        }

        mat.SetFloat("_Number", mat.GetFloat("_Number") - 1);
        yield return new WaitForSeconds(1f/fps);

        for (int i = 4; i >= 0; i--)
        {
            mat.SetFloat("_Index", i);
            yield return new WaitForSeconds(1f/fps);
        }
    }

}
