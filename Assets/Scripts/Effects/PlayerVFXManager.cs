using System;
using System.Collections;
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
    private Material enlightenEye;
    private Coroutine enlightenRoutine;

    //luck
    [SerializeField] private Slider luckSlider;

    public static PlayerVFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        //TODO: Does this need to be DDoL?
        // DontDestroyOnLoad(this.gameObject);
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

        enlightenEye = effects[5].transform.Find("Eye").GetComponent<SpriteRenderer>().material;


        foreach (GameObject obj in effects)
        {
            obj.SetActive(false);
        }
    }

    //DEBUG
    private bool toggle = false;
    public enum Effect
    {
        speed,
        regen,
        strength,
        fortify,
        bless,
        enlighten,
        luck,
        none
    };

    public Effect currentState = Effect.none;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !toggle)
        {
            switch (currentState)
            {
                case Effect.speed:
                    EnableSpeed();
                    break;
                case Effect.regen:
                    EnableRegen();
                    break;
                case Effect.strength:
                    EnableStrength();
                    break;
                case Effect.fortify:
                    EnableFortify();
                    break;
                case Effect.bless:
                    EnableBless();
                    break;
                case Effect.enlighten:
                    EnableEnlighten();
                    break;
                case Effect.luck:
                    EnableLuck();
                    break;
            }
            toggle = !toggle;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            switch (currentState)
            {
                case Effect.speed:
                    DisableSpeed();
                    break;
                case Effect.regen:
                    DisableRegen();
                    break;
                case Effect.strength:
                    DisableStrength();
                    break;
                case Effect.fortify:
                    DisableFortify();
                    break;
                case Effect.bless:
                    DisableBless();
                    break;
                case Effect.enlighten:
                    EnableEnlighten();
                    break;
                case Effect.luck:
                    DisableLuck();
                    break;
            }
            toggle = !toggle;
        }

        if (Input.GetKeyDown(KeyCode.O) && currentState == Effect.luck)
        {
            DecreaseLuckValue(0.25f);
        }
    }

    //-----------------------------------SPEED-----------------------------------
    //manually enable/disable speed
    public void EnableSpeed()
    {
        if (speedPartRoutine != null) StopCoroutine(speedPartRoutine);
        if (speedRingRoutine != null) StopCoroutine(speedRingRoutine);
        effects[0].SetActive(true);
        speedVFX.Play();
        speedRingRoutine = StartCoroutine(FadeInRing(speedRing));
    }

    public void DisableSpeed()
    {
       speedPartRoutine = StartCoroutine(DisableVFX(0));
       StartCoroutine(FadeOutRing(speedRing, speedRI));
    }

    //-----------------------------------REGEN-----------------------------------
    public void EnableRegen()
    {
        if (regenPartRoutine != null) StopCoroutine(regenPartRoutine);
        effects[1].SetActive(true);
        regenVFX.Play();
    }

    public void DisableRegen()
    {
       regenPartRoutine = StartCoroutine(DisableVFX(1));
    }

    //-----------------------------------STRENGTH-----------------------------------
    public void EnableStrength()
    {
        if (strengthPartRoutine != null) StopCoroutine(strengthPartRoutine);
        effects[2].SetActive(true);
        strengthVFX.Play();
    }

    public void DisableStrength()
    {
       strengthPartRoutine = StartCoroutine(DisableVFX(2));
    }

    //-----------------------------------FORTIFY-----------------------------------
    public void EnableFortify()
    {
        if (fortifyPartRoutine != null) StopCoroutine(fortifyPartRoutine);

        if (fortifyAuraRoutine != null) StopCoroutine(fortifyAuraRoutine);
        if (fortifyRingRoutine != null) StopCoroutine(fortifyRingRoutine);

        effects[3].SetActive(true);
        fortifyVFX.Play();
        fortifyRingRoutine = StartCoroutine(FadeInRing(fortifyRing));
        fortifyAuraRoutine = StartCoroutine(FadeInAura(fortifyAuraF, fortifyAuraB, fortifyAuraOff));
    }
    public void DisableFortify()
    {
        fortifyPartRoutine = StartCoroutine(DisableVFX(3));
        fortifyRingRoutine = StartCoroutine(FadeOutRing(fortifyRing, fortifyRI));
        fortifyAuraRoutine = StartCoroutine(FadeOutAura(fortifyAuraF, fortifyAuraB, fortifyAuraOff));
    }

    //-----------------------------------BLESS-----------------------------------
    public void EnableBless()
    {
        if (blessPartRoutine != null) StopCoroutine(blessPartRoutine);

        if (blessAuraRoutine != null) StopCoroutine(blessAuraRoutine);
        if (blessRingRoutine != null) StopCoroutine(blessRingRoutine);

        effects[4].SetActive(true);
        blessVFX.Play();
        blessRingRoutine = StartCoroutine(FadeInRing(blessRing));
        blessAuraRoutine = StartCoroutine(FadeInAura(blessAuraF, blessAuraB, blessAuraOff));
    }

    public void DisableBless()
    {
        if (blessAuraRoutine != null) StopCoroutine(blessAuraRoutine);
        if (blessRingRoutine != null) StopCoroutine(blessRingRoutine);

        blessPartRoutine = StartCoroutine(DisableVFX(4));
        blessRingRoutine = StartCoroutine(FadeOutRing(blessRing, blessRI));
        blessAuraRoutine = StartCoroutine(FadeOutAura(blessAuraF, blessAuraB, blessAuraOff));
    }

    //-----------------------------------ENLIGHTEN-----------------------------------
    public void EnableEnlighten()
    {
        if (enlightenRoutine != null) StopCoroutine(enlightenRoutine);

        effects[5].SetActive(true);
        enlightenRoutine = StartCoroutine(CountDown(enlightenEye));
    }

    private IEnumerator CountDown(Material mat)
    {
        mat.SetFloat("_Index", 0);
        mat.SetInt("_ShowNumber", 1);
        mat.SetFloat("_Number", 5);
        mat.SetFloat("_Alpha", 0f);

        int fps = 24;

        for (int i = 0; i < 11; i++)
        {
            mat.SetFloat("_Alpha", Mathf.Lerp(0f, 1f, i / 11f));
            yield return new WaitForSeconds(1f/fps); //12 frames
        }
        mat.SetFloat("_Alpha", 1f);
        

        yield return new WaitForSeconds(0.5f); // 12 frames


        //loop for rest of
        for (int times = 0; times <= 4; times++)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (i == 4) mat.SetInt("_ShowNumber", 0);
                mat.SetFloat("_Index", i);
                yield return new WaitForSeconds(1f/fps); //5 frames
            }

            mat.SetFloat("_Number", mat.GetFloat("_Number") - 1);
            yield return new WaitForSeconds(3f/fps); //3 frames

            for (int i = 4; i >= 0; i--)
            {
                if (i == 3) mat.SetInt("_ShowNumber", 1);
                mat.SetFloat("_Index", i);
                yield return new WaitForSeconds(1f/fps); //5 frames
            }
            yield return new WaitForSeconds(12f/fps); // 12 frames
        }

        for (int i = 0; i < 11; i++)
        {
            mat.SetFloat("_Alpha", Mathf.Lerp(1f, 0f, i / 11f));
            yield return new WaitForSeconds(1f/fps); //12 frames
        }
        mat.SetFloat("_Alpha", 0f);
        effects[5].SetActive(false);
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
        value = Mathf.Clamp(value, 0f, 1f);
        luckSlider.value = value;
    }

    public void DecreaseLuckValue(float delta)
    {
        float value = Mathf.Clamp(luckSlider.value - delta, 0f, 1f);
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

