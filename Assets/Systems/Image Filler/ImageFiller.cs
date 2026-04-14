using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ImageFillSegment
{
    public Image image;
    [Min(0f)]
    [Tooltip("Relative duration share. Values are normalized to sum to 1. If every weight is 0, time is split equally.")]
    public float weight = 1f;
}

[RequireComponent(typeof(Image))]
public class ImageFiller : MonoBehaviour
{
    [Header("Duration")]
    public DurationSource Source = DurationSource.Manual;
    [ShowIf(nameof(IsManual))]
    public float Duration = 1f;
    [HideIf(nameof(IsManual))]
    public AudioClip audioClip;

    private bool IsManual() => Source == DurationSource.Manual;

    [Header("Sequential fill")]
    [Tooltip("Leave empty (or no valid images) to use only this object's Image for the full duration. Otherwise each entry fills in order; weights control each segment's share of total time.")]
    public List<ImageFillSegment> FillSegments = new List<ImageFillSegment>();

    [Header("Options")]
    public bool FillOnStart;
    public bool Reverse;
    public bool StartAudioAfterDelay;
    public float DelayBeforeFill;

    [Header("Events")]
    public UnityEvent2 OnStarted;
    public UnityEvent2 OnFillStarted;
    public UnityEvent2 OnFillCompleted;

    private Image _image;
    private Coroutine _routine;

    private void Awake()
    {
        _image = GetComponent<Image>();
        ApplyInitialFillToSequence();
    }

    private void Start()
    {
        if (FillOnStart)
            Fill();
    }

    private void BuildFillData(out List<Image> images, out List<float> weights)
    {
        images = new List<Image>();
        weights = new List<float>();

        if (FillSegments != null)
        {
            foreach (var seg in FillSegments)
            {
                if (seg == null || seg.image == null)
                    continue;
                images.Add(seg.image);
                weights.Add(Mathf.Max(0f, seg.weight));
            }
        }

        if (images.Count == 0)
        {
            images.Add(_image);
            weights.Add(1f);
        }
    }

    private void ApplyInitialFillToSequence()
    {
        float initial = Reverse ? 1f : 0f;
        BuildFillData(out var images, out _);
        foreach (var img in images)
        {
            if (img != null)
                img.fillAmount = initial;
        }
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
        ApplyInitialFillToSequence();
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

    private static float[] ComputeSegmentLengths(float totalDuration, int segmentCount, List<float> weights)
    {
        var lengths = new float[segmentCount];
        if (segmentCount <= 0)
            return lengths;

        bool useWeights = weights != null && weights.Count == segmentCount;
        float sum = 0f;
        if (useWeights)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                float w = weights[i];
                if (w > 0f)
                    sum += w;
            }
            useWeights = sum > 0f;
        }

        if (!useWeights)
        {
            float each = totalDuration / segmentCount;
            for (int i = 0; i < segmentCount; i++)
                lengths[i] = each;
            return lengths;
        }

        for (int i = 0; i < segmentCount; i++)
        {
            float w = Mathf.Max(0f, weights[i]);
            lengths[i] = totalDuration * (w / sum);
        }

        return lengths;
    }

    private IEnumerator FillRoutine(float duration)
    {
        if (!StartAudioAfterDelay)
            PlayAudioClip();

        OnStarted?.Invoke();

        yield return new WaitForSeconds(DelayBeforeFill);

        if (StartAudioAfterDelay)
            PlayAudioClip();

        OnFillStarted?.Invoke();

        float from = Reverse ? 1f : 0f;
        float to = Reverse ? 0f : 1f;

        BuildFillData(out List<Image> images, out List<float> weightList);
        int n = images.Count;
        float[] segmentLengths = ComputeSegmentLengths(duration, n, weightList);

        foreach (var img in images)
        {
            if (img != null)
                img.fillAmount = from;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float e = Mathf.Min(elapsed, duration);

            int seg = 0;
            float segStart = 0f;
            while (seg < n && e >= segStart + segmentLengths[seg])
            {
                segStart += segmentLengths[seg];
                seg++;
            }

            if (seg >= n)
            {
                for (int i = 0; i < n; i++)
                {
                    if (images[i] != null)
                        images[i].fillAmount = to;
                }
                break;
            }

            float segLen = segmentLengths[seg];
            float localT = segLen > 0f ? (e - segStart) / segLen : 1f;
            localT = Mathf.Clamp01(localT);

            for (int i = 0; i < n; i++)
            {
                if (images[i] == null) continue;
                if (i < seg)
                    images[i].fillAmount = to;
                else if (i > seg)
                    images[i].fillAmount = from;
                else
                    images[i].fillAmount = Mathf.Lerp(from, to, localT);
            }

            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            if (images[i] != null)
                images[i].fillAmount = to;
        }

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