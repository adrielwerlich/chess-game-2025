using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioClip backgroundMusic;

    private AudioSource audioSource;
    private AudioClip[] musicClips;
    private int currentClipIndex = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();

            // if (backgroundMusic == null)
            // {
            //     backgroundMusic = Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/"); // Ensure you have a BackgroundMusic file in Resources folder
            // }

            audioSource.loop = false;
            audioSource.playOnAwake = true;
            audioSource.volume = 1.0f;

            musicClips = new AudioClip[]
            {
                Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/Darkness"),
                Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/Forest"),
                Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/Happy"),
                Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/Mystery"),
                Resources.Load<AudioClip>("Sound/CasualRelaxingGameMusic/Space Walk")
            };


            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // When the current clip finishes, play the next one
        if (!audioSource.isPlaying && musicClips.Length > 0)
        {
            PlayNextClip();
        }
    }

    private void PlayNextClip()
    {
        audioSource.clip = musicClips[currentClipIndex];
        audioSource.Play();
        currentClipIndex = (currentClipIndex + 1) % musicClips.Length;
    }

}