using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIHoverAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Events")]
    public UnityEvent2 OnHover;

    [Space(10)]

    [Header("Hover For Duration")]
    public bool useHoverForDuration;
    [ShowIf("useHoverForDuration")]
    public float HoverDuration = 1f;
    [ShowIf("useHoverForDuration")]
    public UnityEvent2 OnHoverForDuration;

    [Space(10)]

    public UnityEvent2 OnHoverExit;



    private Interactable _interactable;
    private bool _isHovering;
    private Coroutine _hoverRoutine;

    private bool CanInteract => _interactable == null || _interactable.IsInteractable;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract)
            return;

        _isHovering = true;
        OnHover?.Invoke();

        if (useHoverForDuration)
        {
            if (_hoverRoutine != null)
            {
                StopCoroutine(_hoverRoutine);
                _hoverRoutine = null;
            }
            _hoverRoutine = StartCoroutine(HoverForDurationRoutine());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanInteract)
            return;

        _isHovering = false;
        if (_hoverRoutine != null)
        {
            StopCoroutine(_hoverRoutine);
            _hoverRoutine = null;
        }

        OnHoverExit?.Invoke();
    }

    private System.Collections.IEnumerator HoverForDurationRoutine()
    {
        float t = 0f;
        float duration = Mathf.Max(0f, HoverDuration);

        while (_isHovering && t < duration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        _hoverRoutine = null;

        if (_isHovering && CanInteract)
        {
            OnHoverForDuration?.Invoke();
        }
    }
}