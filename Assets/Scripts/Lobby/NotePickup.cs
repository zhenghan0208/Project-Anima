using TMPro;
using UnityEngine;

public class NotePickup : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public string interactText = "Press E to Read";

    [Header("Note")]
    [TextArea(5, 20)]
    public string noteContent;

    [Header("UI")]
    public NoteUI noteUI;

    private bool collected;

    public void Interact()
    {
        if (collected)
            return;

        if (noteUI == null)
            return;

        collected = true;

        noteUI.ShowNote(noteContent, this);
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public void Hide()
    {
        PlayerInteract player = FindFirstObjectByType<PlayerInteract>();

        if (player != null)
        {
            player.ClearInteractable();
        }

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInteract interact = other.GetComponent<PlayerInteract>();

        if (interact != null)
        {
            interact.SetInteractable(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInteract interact = other.GetComponent<PlayerInteract>();

        if (interact != null)
        {
            interact.ClearInteractable();
        }
    }
}