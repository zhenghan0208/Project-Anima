using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    public TMP_Text noteText;

    public Button noteButton;

    private string currentNote;

    private NotePickup currentPickup;

    void Start()
    {
        panel.SetActive(false);

        noteButton.gameObject.SetActive(false);
    }

    public void ShowNote(string text, NotePickup pickup)
    {
        currentNote = text;
        currentPickup = pickup;

        panel.SetActive(true);

        noteText.text = currentNote;

        Time.timeScale = 0f;
    }

    public void CloseNote()
    {
        panel.SetActive(false);

        noteButton.gameObject.SetActive(true);

        Time.timeScale = 1f;

        if (currentPickup != null)
        {
            currentPickup.Hide();
            currentPickup = null;
        }
    }

    public void OpenCollectedNote()
    {
        panel.SetActive(true);

        noteText.text = currentNote;

        Time.timeScale = 0f;
    }
}