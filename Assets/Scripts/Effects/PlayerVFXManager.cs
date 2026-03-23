using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    /**
        0 - speed
        1 - regen
        2 - strength
        3 - fortify
        4 - bless
        5 - enlighten
        6 - luck
    **/
    [SerializeField] GameObject[] effects;
    [SerializeField] private float fadeTime = 0.5f;
    private bool[] vfxStates;

    //speed
    private Material speedRing;
    [SerializeField] private VisualEffect speedVFX;
    private float speedRI;
    private Coroutine speedRingRoutine;
    private Coroutine speedPartRoutine;
    

    //regen
    [SerializeField] private VisualEffect regenVFX;
    private Coroutine regenPartRoutine;

    //strength
    [SerializeField] private VisualEffect strengthVFX;
    private Coroutine strengthPartRoutine;

    //fortify
    private Material fortifyRing;
    private float fortifyRI;
    private float fortifyAuraOff;
    private Material fortifyAuraF;
    private Material fortifyAuraB;
    [SerializeField] private VisualEffect fortifyVFX;
    private Coroutine fortifyRingRoutine;
    private Coroutine fortifyAuraRoutine;
    private Coroutine fortifyPartRoutine;

    //bless
    private Material blessRing;
    private float blessRI;
    private float blessAuraOff;
    private Material blessAuraF;
    private Material blessAuraB;
    [SerializeField] private VisualEffect blessVFX;
    private Coroutine blessRingRoutine;
    private Coroutine blessAuraRoutine;
    private Coroutine blessPartRoutine;
    private float maxYOff = -2.2f;

    //enlighten
    [SerializeField] TextMeshProUGUI timer;

    //luck
    [SerializeField] private Slider luckSlider;

    void Awake()
    {
        vfxStates = new bool[effects.Length];
    }
    
    void Start()
    {
        speedRing = effects[0].transform.Find("Ring").GetComponent<MeshRenderer>().material;
        speedRI = speedRing.GetFloat("_Remap_Intensity");
        fortifyRing = effects[3].transform.Find("Ring").GetComponent<MeshRenderer>().material;
        fortifyRI = fortifyRing.GetFloat("_Remap_Intensity");
        blessRing = effects[4].transform.Find("Ring").GetComponent<MeshRenderer>().material;
        blessRI = blessRing.GetFloat("_Remap_Intensity");

        fortifyAuraF = effects[3].transform.Find("AuraF").GetComponent<MeshRenderer>().material;
        fortifyAuraB = effects[3].transform.Find("AuraB").GetComponent<MeshRenderer>().material;
        fortifyAuraOff = fortifyAuraF.GetFloat("_Y_Offset");
        blessAuraF = effects[4].transform.Find("AuraF").GetComponent<MeshRenderer>().material;
        blessAuraB = effects[4].transform.Find("AuraB").GetComponent<MeshRenderer>().material;
        blessAuraOff = blessAuraF.GetFloat("_Y_Offset");

        foreach (GameObject obj in effects)
        {
            obj.SetActive(false);
        }
        for (int i = 0; i < effects.Length; i++)
        {
            vfxStates[i] = false;
        }
    }

    //-----------------------------------SPEED-----------------------------------
    //manually enable/disable speed
    public void EnableSpeed()
    {
        if (vfxStates[0]) return;

        if (speedPartRoutine != null) StopCoroutine(speedPartRoutine);
        if (speedRingRoutine != null) StopCoroutine(speedRingRoutine);
        effects[0].SetActive(true);
        speedVFX.Play();
        speedRingRoutine = StartCoroutine(FadeInRing(speedRing));
        vfxStates[0] = true;
    }

    public void DisableSpeed()
    {
        if (!vfxStates[0]) return;
        speedPartRoutine = StartCoroutine(DisableVFX(0));
        StartCoroutine(FadeOutRing(speedRing, speedRI));
        vfxStates[0] = false;
    }

    //-----------------------------------REGEN-----------------------------------
    public void EnableRegen()
    {
        if (vfxStates[1]) return;
        if (regenPartRoutine != null) StopCoroutine(regenPartRoutine);
        effects[1].SetActive(true);
        regenVFX.Play();
        vfxStates[1] = true;
    }

    public void DisableRegen()
    {
        if (!vfxStates[1]) return;
        regenPartRoutine = StartCoroutine(DisableVFX(1));
        vfxStates[1] = false;
    }

    //-----------------------------------STRENGTH-----------------------------------
    public void EnableStrength()
    {
        if (vfxStates[2]) return;
        if (strengthPartRoutine != null) StopCoroutine(strengthPartRoutine);
        effects[2].SetActive(true);
        strengthVFX.Play();
        vfxStates[2] = true;
    }

    public void DisableStrength()
    {
        if (!vfxStates[2]) return;
        strengthPartRoutine = StartCoroutine(DisableVFX(2));
        vfxStates[2] = false;
    }

    //-----------------------------------FORTIFY-----------------------------------
    public void EnableFortify()
    {
        if (vfxStates[3]) return;
        if (fortifyPartRoutine != null) StopCoroutine(fortifyPartRoutine);

        if (fortifyAuraRoutine != null) StopCoroutine(fortifyAuraRoutine);
        if (fortifyRingRoutine != null) StopCoroutine(fortifyRingRoutine);

        effects[3].SetActive(true);
        fortifyVFX.Play();
        fortifyRingRoutine = StartCoroutine(FadeInRing(fortifyRing));
        fortifyAuraRoutine = StartCoroutine(FadeInAura(fortifyAuraF, fortifyAuraB, fortifyAuraOff));
        vfxStates[3] = true;
    }
    public void DisableFortify()
    {
        if (!vfxStates[3]) return;
        fortifyPartRoutine = StartCoroutine(DisableVFX(3));
        fortifyRingRoutine = StartCoroutine(FadeOutRing(fortifyRing, fortifyRI));
        fortifyAuraRoutine = StartCoroutine(FadeOutAura(fortifyAuraF, fortifyAuraB, fortifyAuraOff));
        vfxStates[3] = false;
    }

    //-----------------------------------BLESS-----------------------------------
    public void EnableBless()
    {
        if (vfxStates[4]) return;
        if (blessPartRoutine != null) StopCoroutine(blessPartRoutine);

        if (blessAuraRoutine != null) StopCoroutine(blessAuraRoutine);
        if (blessRingRoutine != null) StopCoroutine(blessRingRoutine);

        effects[4].SetActive(true);
        blessVFX.Play();
        blessRingRoutine = StartCoroutine(FadeInRing(blessRing));
        blessAuraRoutine = StartCoroutine(FadeInAura(blessAuraF, blessAuraB, blessAuraOff));
        vfxStates[4] = true;
    }

    public void DisableBless()
    {
        if (!vfxStates[4]) return;
        if (blessAuraRoutine != null) StopCoroutine(blessAuraRoutine);
        if (blessRingRoutine != null) StopCoroutine(blessRingRoutine);

        blessPartRoutine = StartCoroutine(DisableVFX(4));
        blessRingRoutine = StartCoroutine(FadeOutRing(blessRing, blessRI));
        blessAuraRoutine = StartCoroutine(FadeOutAura(blessAuraF, blessAuraB, blessAuraOff));
        vfxStates[4] = false;
    }

    //-----------------------------------ENLIGHTEN-----------------------------------
    public void EnableEnlighten()
    {
        effects[5].SetActive(true);
    }

    public void DisableEnlighten()
    {
        effects[5].SetActive(false);
    }

    public void SetEnlightenTime(float time)
    {
        int intTime = Mathf.CeilToInt(time);
        timer.SetText(intTime.ToString());
    }

    //-----------------------------------LUCK-----------------------------------
    public void EnableLuck()
    {
        luckSlider.value = 1f;
        effects[6].SetActive(true);
    }

    public void DisableLuck()
    {
        effects[6].SetActive(false);
    }

    public void SetLuckValue(float value)
    {
        if (!effects[6].activeSelf) return;
        luckSlider.value = value;
    }

    //-----------------------------------Shared Functions-----------------------------------
    private IEnumerator FadeInRing(Material mat)
    {
        mat.SetFloat("_Remap_Intensity", 1f);
        mat.SetFloat("_Alpha", 0f);

        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            mat.SetFloat("_Remap_Intensity", Mathf.Lerp(1f, speedRI, elapsedTime / fadeTime));
            mat.SetFloat("_Alpha", Mathf.Lerp(0f, 1f, elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        mat.SetFloat("_Remap_Intensity", speedRI);
        mat.SetFloat("_Alpha", 1f);
        yield return null;
    }

    private IEnumerator FadeOutRing(Material mat, float remapIntensity)
    {
        mat.SetFloat("_Remap_Intensity", remapIntensity);
        mat.SetFloat("_Alpha", 1f);

        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            mat.SetFloat("_Remap_Intensity", Mathf.Lerp(remapIntensity, 1f, elapsedTime / fadeTime));
            mat.SetFloat("_Alpha", Mathf.Lerp(1f, 0f, elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        mat.SetFloat("_Remap_Intensity", 1f);
        mat.SetFloat("_Alpha", 0f);
        yield return null;
    }

    private IEnumerator FadeInAura(Material front, Material back, float yOffset)
    {
        front.SetFloat("_Y_Offset", maxYOff);
        back.SetFloat("_Y_Offset", maxYOff);

        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            front.SetFloat("_Y_Offset", Mathf.Lerp(maxYOff, yOffset, elapsedTime / fadeTime));
            back.SetFloat("_Y_Offset", Mathf.Lerp(maxYOff, yOffset, elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        front.SetFloat("_Y_Offset", yOffset);
        back.SetFloat("_Y_Offset", yOffset);
        
    }

    private IEnumerator FadeOutAura(Material front, Material back, float yOffset)
    {
        front.SetFloat("_Y_Offset", yOffset);
        back.SetFloat("_Y_Offset", yOffset);
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            front.SetFloat("_Y_Offset", Mathf.Lerp(yOffset, maxYOff, elapsedTime / fadeTime));
            back.SetFloat("_Y_Offset", Mathf.Lerp(yOffset, maxYOff, elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  

        front.SetFloat("_Y_Offset", maxYOff);
        back.SetFloat("_Y_Offset", maxYOff);
        
    }

    private IEnumerator DisableVFX(int index)
    {
        switch (index) {
            case 0:
                speedVFX.Stop();
                break;
            case 1:
                regenVFX.Stop();
                break;
            case 2:
                strengthVFX.Stop();
                break;
            case 3:
                fortifyVFX.Stop();
                break;
            case 4:
                blessVFX.Stop();
                break;
        }
        yield return new WaitForSeconds(3f);
        effects[index].SetActive(false);
    }
}

