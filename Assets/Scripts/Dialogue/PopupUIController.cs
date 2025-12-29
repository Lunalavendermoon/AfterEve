using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    //[SerializeField] private Image popupPortrait;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private float displayTime = 2.0f;

    public bool isActive { get; private set; }

    private Coroutine currentRoutine;

    private void Start()
    {
        canvasGroup.alpha = 0;
    }

    public void Show(string message)
    {
        if (isActive) return;

        currentRoutine = StartCoroutine(ShowCoroutine(message));
    }

    private IEnumerator ShowCoroutine(string message)
    {
        isActive = true;
        popupText.text = message;
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        isActive = false;
        popupText.text = "";
        canvasGroup.alpha = 0;
    }
}
