using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Graphic))]
public class GraphicController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Graphic graphic;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Defaults")]
    [SerializeField] private bool useUnscaledTime;
    [Tooltip("If CanvasGroup exists, prefer controlling alpha through it (instead of Graphic.color.a).")]
    [SerializeField] private bool preferCanvasGroupForAlpha = true;

    [Header("Raycast / Interactable (CanvasGroup)")]
    [SerializeField] private bool autoToggleRaycastTarget = true;

    private Coroutine _activeRoutine;

    private void Awake()
    {
        CacheRefs();
    }

    private void Reset()
    {
        CacheRefs();
    }

    private void OnValidate()
    {
        if (!graphic)
            graphic = GetComponent<Graphic>();
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void CacheRefs()
    {
        if (!graphic)
            graphic = GetComponent<Graphic>();
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    // ---- Instant setters ----

    public void SetColor(Color color)
    {
        CacheRefs();
        if (!graphic) return;
        graphic.color = color;
    }

    public void SetAlpha(float alpha)
    {
        CacheRefs();
        alpha = Mathf.Clamp01(alpha);

        if (preferCanvasGroupForAlpha && canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else if (graphic != null)
        {
            var c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }

        ApplyRaycastPolicy(alpha);
    }

    public void SetVisible(bool visible)
    {
        SetAlpha(visible ? 1f : 0f);
    }

    // ---- Timed transitions ----

    public void FadeTo(float targetAlpha, float duration)
    {
        CacheRefs();
        targetAlpha = Mathf.Clamp01(targetAlpha);

        Stop();

        float from = GetCurrentAlpha();
        if (duration <= 0f || Mathf.Abs(from - targetAlpha) <= 0.0001f)
        {
            SetAlpha(targetAlpha);
            return;
        }

        _activeRoutine = StartCoroutine(LerpAlpha(from, targetAlpha, duration));
    }

    public void FadeFromTo(float from_alpha, float target_alpha, float duration)
    {
        CacheRefs();
        from_alpha = Mathf.Clamp01(from_alpha);
        target_alpha = Mathf.Clamp01(target_alpha);

        Stop();
        SetAlpha(from_alpha);

        if (duration <= 0f || Mathf.Abs(from_alpha - target_alpha) <= 0.0001f)
        {
            SetAlpha(target_alpha);
            return;
        }

        _activeRoutine = StartCoroutine(LerpAlpha(from_alpha, target_alpha, duration, null));
    }

    public void ColorTo(Color targetColor, float duration = -1f, Action onComplete = null)
    {
        CacheRefs();
        if (!graphic) return;

        Stop();

        Color from = graphic.color;
        if (duration <= 0f || from.Equals(targetColor))
        {
            graphic.color = targetColor;
            onComplete?.Invoke();
            return;
        }

        _activeRoutine = StartCoroutine(LerpColor(from, targetColor, duration, onComplete));
    }

    public void ChangeColorDuration(Color targetColor, float duration)
    {
        ColorTo(targetColor, duration);
    }

    public void AlphaAndColorTo(float targetAlpha, Color targetColor, float duration = -1f, Action onComplete = null)
    {
        CacheRefs();
        if (!graphic) return;

        targetAlpha = Mathf.Clamp01(targetAlpha);

        Stop();

        float fromA = GetCurrentAlpha();
        Color fromC = graphic.color;

        _activeRoutine = StartCoroutine(LerpAlphaAndColor(fromA, targetAlpha, fromC, targetColor, duration, onComplete));
    }

    // ---- CanvasGroup helpers ----

    public void SetCanvasGroupInteractable(bool interactable)
    {
        CacheRefs();
        if (canvasGroup == null) return;

        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    // ---- Stop ----

    public void Stop()
    {
        if (_activeRoutine != null)
        {
            StopCoroutine(_activeRoutine);
            _activeRoutine = null;
        }
    }

    // ---- Internals ----

    private float GetCurrentAlpha()
    {
        if (preferCanvasGroupForAlpha && canvasGroup != null)
            return canvasGroup.alpha;
        if (graphic != null)
            return graphic.color.a;
        return 1f;
    }

    private float Dt => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    private IEnumerator LerpAlpha(float from, float to, float duration, Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Dt;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            SetAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetAlpha(to);
        _activeRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator LerpColor(Color from, Color to, float duration, Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Dt;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            if (graphic != null)
                graphic.color = Color.Lerp(from, to, t);
            yield return null;
        }

        if (graphic != null)
            graphic.color = to;
        _activeRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator LerpAlphaAndColor(float fromA, float toA, Color fromC, Color toC, float duration, Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Dt;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            float a = Mathf.Lerp(fromA, toA, t);
            Color c = Color.Lerp(fromC, toC, t);

            if (graphic != null)
                graphic.color = c;
            SetAlpha(a);

            yield return null;
        }

        if (graphic != null)
            graphic.color = toC;
        SetAlpha(toA);
        _activeRoutine = null;
        onComplete?.Invoke();
    }

    private void ApplyRaycastPolicy(float alpha)
    {
        if (!autoToggleRaycastTarget)
            return;

        if (graphic != null)
            graphic.raycastTarget = alpha > 0.0001f;

        if (canvasGroup != null)
        {
            bool on = alpha > 0.0001f;
            canvasGroup.blocksRaycasts = on;
            if (!on) canvasGroup.interactable = false;
        }
    }
}