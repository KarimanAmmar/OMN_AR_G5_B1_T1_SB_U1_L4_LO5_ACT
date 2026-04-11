using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFiller : MonoBehaviour
{


    [Header("Duration")]
    public DurationSource Source = DurationSource.Manual;
    public float Duration = 1f;
    public AudioClip audioClip;

    [Header("Options")]
    public bool FillOnStart;
    public bool Reverse;
    public float DelayBeforeStartFill;
    public float DelayBeforeStartAudio;

    [Header("Events")]
    public UnityEvent2 OnStarted;
    public UnityEvent2 OnFillStarted;
    public UnityEvent2 OnAudioStarted;
    public UnityEvent2 OnFillCompleted;

    private Image _image;
    private Coroutine _routine;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.fillAmount = Reverse ? 1f : 0f;
    }

    private void Start()
    {
        if (FillOnStart)
            Fill();
    }

    public void Fill()
    {
        gameObject.SetActive(true);
        Stop();
        float dur = GetDuration();
        _routine = StartCoroutine(FillRoutine(dur));
    }

    public void ResetFill()
    {
        Stop();
        _image.fillAmount = Reverse ? 1f : 0f;
    }

    public void Stop()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;

            AudioManager.Instance.StopAllSFX();
        }
    }

    private float GetDuration()
    {
        if (Source == DurationSource.AudioClip && audioClip != null)
            return audioClip.length;
        return Mathf.Max(Duration, 0.01f);
    }

    private IEnumerator FillRoutine(float duration)
    {
        OnStarted?.Invoke();

        yield return new WaitForSeconds(DelayBeforeStartFill);

        OnFillStarted?.Invoke();

        yield return new WaitForSeconds(DelayBeforeStartAudio);

        PlayAudioClip();

        OnAudioStarted?.Invoke();


        float from = Reverse ? 1f : 0f;
        float to = Reverse ? 0f : 1f;
        float elapsed = 0f;

        _image.fillAmount = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _image.fillAmount = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        _image.fillAmount = to;
        _routine = null;
        OnFillCompleted?.Invoke();
    }


    public void PlayAudioClip()
    {
        if (audioClip == null)
            return;

        AudioManager.Instance.PlaySFX(audioClip);
    }





    public enum DurationSource
    {
        Manual, AudioClip
    }
}
