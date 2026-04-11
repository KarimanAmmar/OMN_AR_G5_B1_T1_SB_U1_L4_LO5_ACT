using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;


[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class SceneFader : MonoBehaviour
{
    [Header("Fade In")]
    [SerializeField] private bool fadeInOnStart;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeInDelayBefore;
    [SerializeField] private float fadeInDelayAfter;


    [Header("Fade Out")]
    [SerializeField] private bool fadeOutOnStart;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float fadeOutDelayBefore;
    [SerializeField] private float fadeOutDelayAfter;


    private Image image;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;


    public static SceneFader Instance;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GetComponents();
    }


    private void Start()
    {
        if (fadeInOnStart)
        {
            FadeIn();
        }
        if (fadeOutOnStart)
        {
            FadeOut();
        }
    }


    public void FadeIn(System.Action onFadeCompletedCallback = null)
    {
        transform.SetAsLastSibling();

        StartCoroutine(FadeInCoroutine());

        IEnumerator FadeInCoroutine()
        {
            Show();

            yield return new WaitForSeconds(fadeInDelayBefore);

            float time_elapsed = 0;
            while (time_elapsed < fadeInDuration)
            {
                float t = time_elapsed / fadeInDuration;
                canvasGroup.alpha = t;

                yield return null;

                time_elapsed += Time.deltaTime;
            }

            canvasGroup.alpha = 1;

            yield return new WaitForSeconds(fadeInDelayAfter);

            onFadeCompletedCallback?.Invoke();
        }
    }


    public void FadeOut(System.Action onFadeCompletedCallback = null)
    {
        transform.SetAsLastSibling();

        StartCoroutine(FadeOutCoroutine());

        IEnumerator FadeOutCoroutine()
        {
            Show();

            yield return new WaitForSeconds(fadeOutDelayBefore);

            float time_elapsed_reversed = 0;
            while (time_elapsed_reversed < fadeOutDuration)
            {
                float t = time_elapsed_reversed / fadeOutDuration;
                canvasGroup.alpha = 1 - t;

                yield return null;

                time_elapsed_reversed += Time.deltaTime;
            }

            yield return new WaitForSeconds(fadeOutDelayAfter);

            onFadeCompletedCallback?.Invoke();
            Hide();
        }
    }


    [Button("Show")]
    public void Show()
    {
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
        if (!image)
            image = GetComponent<Image>();

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        image.raycastTarget = true;
    }


    [Button("Hide")]
    public void Hide()
    {
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
        if (!image)
            image = GetComponent<Image>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        image.raycastTarget = false;
    }


    private void GetComponents()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }


    private void Reset()
    {
        GetComponents();
        image.color = Color.black;

        canvasGroup.alpha = 1;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        rectTransform.localScale = Vector3.one * 1.5f;
    }
}