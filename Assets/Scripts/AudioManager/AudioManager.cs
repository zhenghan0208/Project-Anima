using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Tooltip("3D audio source for environmental sounds.")]
    public AudioSource environmentSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("SFX")]
    public AudioClip buttonClickSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip jumpSFX;
    public AudioClip dieSFX;
    public AudioClip hurtSFX;
    public AudioClip pickUpSFX;
    public AudioClip attackSFX;
    public AudioClip destroySFX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadVolume();
    }

    void Start()
    {
        ApplyVolume();
    }

    // =========================
    // Music
    // =========================

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // =========================
    // Normal SFX
    // =========================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    // =========================
    // Environment 3D SFX
    // =========================

    public void PlayEnvironmentSFX(
        AudioClip clip,
        Vector3 position)
    {
        if (clip == null)
            return;

        if (environmentSource == null)
            return;

        environmentSource.transform.position = position;

        environmentSource.PlayOneShot(clip);
    }

    // =========================
    // Volume
    // =========================

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        ApplyVolume();

        PlayerPrefs.SetFloat(
            "MusicVolume",
            musicVolume
        );

        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        ApplyVolume();

        PlayerPrefs.SetFloat(
            "SFXVolume",
            sfxVolume
        );

        PlayerPrefs.Save();
    }

    void ApplyVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        if (environmentSource != null)
        {
            environmentSource.volume = sfxVolume;
        }
    }

    void LoadVolume()
    {
        musicVolume =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                1f
            );

        sfxVolume =
            PlayerPrefs.GetFloat(
                "SFXVolume",
                1f
            );
    }
}