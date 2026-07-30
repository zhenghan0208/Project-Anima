using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip music;

    void Start()
    {
        AudioManager.Instance.PlayMusic(music);
    }
}