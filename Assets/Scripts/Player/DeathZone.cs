using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -1)
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex);
        }
    }
}