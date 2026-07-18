using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    public string sceneName;

    [Header("UI")]
    public string interactText = "Press E to Enter";

    public void Interact()
    {
        SceneManager.LoadScene(sceneName);
    }

    public string GetInteractText()
    {
        return interactText;
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