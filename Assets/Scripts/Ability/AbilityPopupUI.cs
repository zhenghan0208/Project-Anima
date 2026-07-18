using System.Collections;
using TMPro;
using UnityEngine;

public class AbilityPopupUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float fadeDuration = 0.3f;
    public float displayTime = 2f;

    // TODO
    // AudioSource
    // AudioClip

    private Coroutine popupCoroutine;

    public void ShowPopup(string message)
    {
        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        popupCoroutine = StartCoroutine(PopupRoutine(message));
    }

    IEnumerator PopupRoutine(string message)
    {
        popupPanel.SetActive(true);

        popupText.text = message;

        canvasGroup.alpha = 0;

        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha = timer / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        timer = fadeDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            canvasGroup.alpha = timer / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 0;

        popupPanel.SetActive(false);

        popupCoroutine = null;
    }
}