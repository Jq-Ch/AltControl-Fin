using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float volume = 0.6f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        SetupAudioSource();
        PlayBGM();
    }

    void SetupAudioSource()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D “Ù¿÷
    }

    public void PlayBGM()
    {
        if (audioSource.clip == null) return;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopBGM()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        audioSource.volume = volume;
    }
}
