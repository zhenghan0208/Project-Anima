using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text interactText;

    void Start()
    {
        Hide();
    }

    public void Show(string message)
    {
        panel.SetActive(true);
        interactText.text = message;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}