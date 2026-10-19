using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager main;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    [SerializeField]
    private List<MusicTape> tapes = new();

    private int currentTapeIndex = 0;

    private int unlockedTemporary = 0;

    [SerializeField]
    private float bigVolume = 2f;

    private int unlockedTapes = 0;

    private AudioSource currentAudioSource;
    private MusicTape currentTape;

    private AudioSource currentMusic;

    private List<AudioFade> fades = new();

    [SerializeField]
    private AudioMixerGroup bigMixer;
    [SerializeField]
    private AudioMixerGroup defaultMixer;

    private PlayerMovement playerMovement;


    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    void Start()
    {
    }

    public void PlayTape(MusicTape tape)
    {
        FadePrevious();
        currentAudioSource = Initialize3DAudioSource(tape);
        currentTape = tape;
        Fade(currentAudioSource, tape.Volume);
    }

    public void UnlockTape()
    {
        unlockedTemporary += 1;
        unlockedTapes += 1;
        if (unlockedTapes == 1)
        {
            PlayMusic();
        } else
        {
            PlayNextTape();
        }
    }

    public void Die()
    {
        unlockedTapes -= unlockedTemporary;
        unlockedTemporary = 0;
        PlayMusic();
    }

    public void PassLevel()
    {
        unlockedTemporary = 0;
    }

    public void PlayMusic()
    {
        if (unlockedTapes == 0)
        {
            PauseMusic();
            return;
        }
        if (currentTapeIndex >= unlockedTapes)
        {
            currentTapeIndex = unlockedTapes - 1;
        }
        if (currentTapeIndex >= tapes.Count)
        {
            currentTapeIndex = 0;
        }
        MusicTape tape = tapes[currentTapeIndex];
        PlayTape(tape);
    }

    public void PlayNextTape()
    {
        currentTapeIndex += 1;
        PlayMusic();
    }

    public void PauseMusic()
    {
        if (currentAudioSource != null)
        {
            currentAudioSource.Pause();
        }
    }

    public void IncreaseVolume()
    {
        //Fade(currentAudioSource, bigVolume, 1f, 1f, false, false);
        currentAudioSource.outputAudioMixerGroup = bigMixer;
    }

    public void DecreaseVolume()
    {
        currentAudioSource.outputAudioMixerGroup = defaultMixer;
        //Fade(currentAudioSource, currentTape.Volume, 1f, 1f, false, false);
    }

    private void FadePrevious()
    {
        if (currentMusic != null && currentMusic.isPlaying)
        {
            Fade(currentMusic, 0, 0.02f, 1f, true, false);
        }
        if (currentAudioSource != null && currentAudioSource.isPlaying)
        {
            Fade(currentAudioSource, 0, 0.02f, 1f, false, false);
        }
    }

    private AudioSource Initialize3DAudioSource(MusicTape tape)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab);
        audioSource.gameObject.transform.parent = transform;
        audioSource.spatialBlend = 1f;
        audioSource.clip = tape.Audio;
        audioSource.volume = 0;
        audioSource.name = $"MusicTape [{tape.SongName}]";
        return audioSource;
    }


    private void Fade(
        AudioSource fadeSource,
        float targetVolume,
        float duration = 0.5f,
        float targetPitch = 1.0f,
        bool useUnscaledTime = false,
        bool destroyAfterFade = false
    )
    {
        AudioFade fade = new(duration, targetVolume, fadeSource, targetPitch, useUnscaledTime, destroyAfterFade);
        fades.Add(fade);
    }

    void Update()
    {
        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
        if (playerMovement == null)
        {
            var player = FindAnyObjectByType<WorldMapPlayerCharacter>();
            if (player)
            {
                currentAudioSource.transform.position = player.transform.position;
            }
        }
        if (playerMovement != null && currentAudioSource != null)
        {
            currentAudioSource.transform.position = playerMovement.transform.position;
        }
        for (int index = 0; index < fades.Count; index += 1)
        {
            AudioFade fade = fades[index];
            if (fade != null && fade.IsFading)
            {
                fade.Update();
            }
            if (!fade.IsFading)
            {
                fades.Remove(fade);
                if (fade.DestroyAfterFade)
                {
                    Destroy(fade.AudioSource.gameObject);
                }
            }
        }
        /*if (Keyboard.current[Key.O].wasPressedThisFrame)
        {
            Debug.Log("Key Pressed O");
            PlayTapeAtPosition(tapes[0], new(1f, 20f, 50f));
        }
        if (Keyboard.current[Key.P].wasPressedThisFrame)
        {
            Debug.Log("Key Pressed P");
            PlayTapeAtPosition(tapes[1], new(50f, -20f, -50f));
        }*/

    }

}

public class AudioFade
{
    public AudioFade(float duration, float target, AudioSource track, float targetPitch, bool useUnscaledTime = false, bool destroyAfterFade = false)
    {
        if (!track.isPlaying)
        {
            track.Play();
        }
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;

        originalPitch = track.pitch;
        this.targetPitch = targetPitch;
        this.useUnscaledTime = useUnscaledTime;
        this.destroyAfterFade = destroyAfterFade;
    }
    public bool IsFading { get; private set; }
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }
    private float originalVolume;

    private bool useUnscaledTime;
    private bool destroyAfterFade;
    public bool DestroyAfterFade { get { return destroyAfterFade; } }

    private float originalPitch, targetPitch;

    public void Update()
    {
        if (useUnscaledTime)
        {
            timer += Time.unscaledDeltaTime / duration;
        }
        else
        {
            timer += Time.deltaTime / duration;
        }
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        audioSource.pitch = Mathf.Lerp(originalPitch, targetPitch, timer);
        if (timer >= 1)
        {
            audioSource.volume = targetVolume;
            audioSource.pitch = targetPitch;
            IsFading = false;
        }
    }
}