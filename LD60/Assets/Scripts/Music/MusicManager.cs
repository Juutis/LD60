using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager main;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    [SerializeField]
    private List<MusicTape> tapes = new();

    private AudioSource currentTape;

    private AudioSource currentMusic;

    private List<AudioFade> fades = new();


    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public void PlayTapeAtPosition(MusicTape tape, Vector3 position)
    {
        FadePrevious();
        currentTape = Initialize3DAudioSource(tape, position);
        Fade(currentTape, tape.Volume);
    }

    public void PlayTape2D(MusicTape tape)
    {
        FadePrevious();
        currentTape = InitializeAudioSource(tape);
        Fade(currentTape, tape.Volume);
    }

    private void FadePrevious()
    {
        if (currentMusic != null && currentMusic.isPlaying)
        {
            Fade(currentMusic, 0, 0.02f, 1f, true, false);
        }
        if (currentTape != null && currentTape.isPlaying)
        {
            Fade(currentTape, 0, 0.02f, 1f, false, true);
        }
    }

    private AudioSource Initialize3DAudioSource(MusicTape tape, Vector3 position)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab);
        audioSource.gameObject.transform.parent = transform;
        audioSource.gameObject.transform.position = position;
        audioSource.spatialBlend = 1f;
        audioSource.clip = tape.Audio;
        audioSource.volume = 0;
        audioSource.name = $"MusicTape [{tape.SongName}]";
        return audioSource;
    }

    private AudioSource InitializeAudioSource(MusicTape tape)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab);
        audioSource.gameObject.transform.parent = transform;
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