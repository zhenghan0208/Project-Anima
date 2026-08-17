using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(PlayClickSFX);
    }

    void PlayClickSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSFX);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSFX();
    }

    void PlayHoverSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonHoverSFX);
        }
    }
}