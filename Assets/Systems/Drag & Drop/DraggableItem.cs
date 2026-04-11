using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drop Settings")]
    public Transform DropTarget;
    public float SnapDistance = 50f;
    public float ReturnDuration = 0.25f;

    [Header("Events")]
    public UnityEvent2 OnHoverEnter;
    public UnityEvent2 OnHoverExit;
    public UnityEvent2 OnStartDrag;
    public UnityEvent2 OnEndedDrag;
    public UnityEvent2 OnDragEachFrame;
    public UnityEvent2 OnDropCorrect;
    public UnityEvent2 OnDropWrong;

    private RectTransform _rect;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    private bool _isDragging;
    private bool _isSnapped;
    private Interactable _interactable;

    private bool CanInteract => _interactable == null || _interactable.IsInteractable;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _interactable = GetComponent<Interactable>();
        _originalPosition = _rect.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract) return;
        if (!_isDragging && !_isSnapped)
            OnHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isDragging && !_isSnapped)
            OnHoverExit?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanInteract || _isSnapped) return;

        _isDragging = true;
        _canvasGroup.blocksRaycasts = false;
        OnStartDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        float scaleFactor = _canvas != null ? _canvas.scaleFactor : 1f;
        _rect.anchoredPosition += eventData.delta / scaleFactor;
        OnDragEachFrame?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        _isDragging = false;
        _canvasGroup.blocksRaycasts = true;

        OnEndedDrag?.Invoke();

        if (DropTarget != null && IsCloseEnough())
        {
            _rect.position = DropTarget.position;
            _isSnapped = true;
            OnDropCorrect?.Invoke();
        }
        else
        {
            OnDropWrong?.Invoke();
            StartCoroutine(ReturnToOrigin());
        }
    }

    private bool IsCloseEnough()
    {
        Vector2 itemScreen = RectTransformUtility.WorldToScreenPoint(null, _rect.position);
        Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(null, DropTarget.position);
        return Vector2.Distance(itemScreen, targetScreen) <= SnapDistance;
    }

    private IEnumerator ReturnToOrigin()
    {
        Vector2 start = _rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < ReturnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / ReturnDuration);
            _rect.anchoredPosition = Vector2.Lerp(start, _originalPosition, t);
            yield return null;
        }

        _rect.anchoredPosition = _originalPosition;
    }

    public void ResetItem()
    {
        _isSnapped = false;
        _rect.anchoredPosition = _originalPosition;
    }
}
