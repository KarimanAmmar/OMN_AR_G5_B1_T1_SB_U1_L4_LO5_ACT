using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume")]
    [Range(0f, 1f)] public float MasterVolume = 1f;
    [Range(0f, 1f)] public float MusicVolume = 1f;
    [Range(0f, 1f)] public float SFXVolume = 1f;

    [Header("Music")]
    public bool PlayMusicOnStart;
    public AudioClip StartMusic;
    public bool LoopMusic = true;

    [Header("Events")]
    public UnityEvent OnMusicStarted;
    public UnityEvent OnMusicStopped;
    public UnityEvent OnMusicCompleted;

    public AudioSource _musicSource;
    public AudioSource _sfxSource;

    private readonly List<AudioSource> _activeSfx = new();
    private Coroutine _fadeCoroutine;
    private Coroutine _musicEndCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (PlayMusicOnStart && StartMusic != null)
            PlayMusic(StartMusic);
    }

    private void Update()
    {
        _musicSource.volume = MasterVolume * MusicVolume;
        _sfxSource.volume = MasterVolume * SFXVolume;
    }


    private void Reset()
    {
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
        }
    }


    // ─── Music ───

    public void PlayMusic(AudioClip clip)
    {
        StopFade();
        StopMusicEndCheck();
        _musicSource.clip = clip;
        _musicSource.loop = LoopMusic;
        _musicSource.volume = MasterVolume * MusicVolume;
        _musicSource.Play();
        OnMusicStarted?.Invoke();

        if (!LoopMusic)
            _musicEndCoroutine = StartCoroutine(WaitForMusicEnd());
    }

    public void StopMusic()
    {
        StopFade();
        StopMusicEndCheck();
        _musicSource.Stop();
        OnMusicStopped?.Invoke();
    }

    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    public void ResumeMusic()
    {
        _musicSource.UnPause();
    }

    public void SetMusicLoop(bool loop)
    {
        LoopMusic = loop;
        _musicSource.loop = loop;
    }

    public void FadeInMusic(AudioClip clip, float duration)
    {
        StopFade();
        StopMusicEndCheck();
        _musicSource.clip = clip;
        _musicSource.loop = LoopMusic;
        _musicSource.volume = 0f;
        _musicSource.Play();
        OnMusicStarted?.Invoke();
        _fadeCoroutine = StartCoroutine(FadeRoutine(0f, MasterVolume * MusicVolume, duration));

        if (!LoopMusic)
            _musicEndCoroutine = StartCoroutine(WaitForMusicEnd());
    }

    public void FadeOutMusic(float duration)
    {
        StopFade();
        _fadeCoroutine = StartCoroutine(FadeOutAndStop(duration));
    }

    public void CrossfadeMusic(AudioClip newClip, float duration)
    {
        StopFade();
        _fadeCoroutine = StartCoroutine(CrossfadeRoutine(newClip, duration));
    }

    // ─── SFX ───

    public void PlaySFX(AudioClip clip)
    {
        StopAllSFX();

        _sfxSource.PlayOneShot(clip, MasterVolume * SFXVolume);
    }

    public void PlaySFX(AudioClip clip, bool stop_other_sfx)
    {
        if (stop_other_sfx)
            StopAllSFX();

        _sfxSource.PlayOneShot(clip, MasterVolume * SFXVolume);
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        StopAllSFX();

        _sfxSource.PlayOneShot(clip, MasterVolume * SFXVolume * volumeScale);
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
    {
        StopAllSFX();

        AudioSource.PlayClipAtPoint(clip, position, MasterVolume * SFXVolume);
    }

    public void PlayRandomSFX(List<AudioClip> clips)
    {
        StopAllSFX();

        int rnd = Random.Range(0, clips.Count);
        _sfxSource.PlayOneShot(clips[rnd]);
    }

    public AudioSource PlaySFXLooped(AudioClip clip)
    {
        StopAllSFX();

        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.volume = MasterVolume * SFXVolume;
        src.Play();
        _activeSfx.Add(src);
        return src;
    }

    public void StopSFXLooped(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        _activeSfx.Remove(source);
        Destroy(source);
    }

    public void StopAllSFX()
    {
        _sfxSource.Stop();
        foreach (var src in _activeSfx)
        {
            if (src != null)
            {
                src.Stop();
                Destroy(src);
            }
        }
        _activeSfx.Clear();
    }

    public void PlaySFXDelayed(AudioClip clip, float delay)
    {
        StartCoroutine(PlaySFXDelayedRoutine(clip, delay));
    }

    // ─── Volume ───

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
    }

    public void Mute()
    {
        MasterVolume = 0f;
    }

    public void Unmute(float volume = 1f)
    {
        MasterVolume = Mathf.Clamp01(volume);
    }

    // ─── State ───

    public bool IsMusicPlaying => _musicSource.isPlaying;
    public AudioClip CurrentMusic => _musicSource.clip;
    public float MusicTime => _musicSource.time;

    // ─── Internal ───

    private IEnumerator PlaySFXDelayedRoutine(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(clip);
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        _musicSource.volume = to;
        _fadeCoroutine = null;
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float start = _musicSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(start, 0f, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        _musicSource.Stop();
        _musicSource.volume = 0f;
        _fadeCoroutine = null;
        OnMusicStopped?.Invoke();
    }

    private IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
    {
        StopMusicEndCheck();
        float startVol = _musicSource.volume;

        float elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVol, 0f, Mathf.Clamp01(elapsed / (duration * 0.5f)));
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.loop = LoopMusic;
        _musicSource.volume = 0f;
        _musicSource.Play();
        OnMusicStarted?.Invoke();

        float targetVol = MasterVolume * MusicVolume;
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0f, targetVol, Mathf.Clamp01(elapsed / (duration * 0.5f)));
            yield return null;
        }
        _musicSource.volume = targetVol;
        _fadeCoroutine = null;

        if (!LoopMusic)
            _musicEndCoroutine = StartCoroutine(WaitForMusicEnd());
    }

    private IEnumerator WaitForMusicEnd()
    {
        yield return new WaitWhile(() => _musicSource.isPlaying);
        _musicEndCoroutine = null;
        OnMusicCompleted?.Invoke();
    }

    private void StopFade()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }

    private void StopMusicEndCheck()
    {
        if (_musicEndCoroutine != null)
        {
            StopCoroutine(_musicEndCoroutine);
            _musicEndCoroutine = null;
        }
    }
}
