using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public InteractUI interactUI;

    private IInteractable currentInteractable;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;

        if (interactUI != null)
        {
            interactUI.Show(interactable.GetInteractText());
        }
    }

    public void ClearInteractable()
    {
        currentInteractable = null;

        if (interactUI != null)
        {
            interactUI.Hide();
        }
    }
}