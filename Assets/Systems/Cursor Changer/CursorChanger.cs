using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IDragHandler, IEndDragHandler
{
    [Header("Cursor Textures")]
    public Texture2D HoverCursor;
    public Texture2D ClickCursor;

    [Header("Hotspot")]
    [Tooltip("Pixel offset from top-left corner used as the click point")]
    public Vector2 HoverHotspot;
    public Vector2 ClickHotspot;

    private bool _hovering;
    private bool _pressed;
    private Interactable _interactable;

    private bool CanInteract => _interactable == null || _interactable.IsInteractable;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract) return;
        _hovering = true;
        if (!_pressed && HoverCursor != null)
            Cursor.SetCursor(HoverCursor, HoverHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
        if (!_pressed)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanInteract) return;
        _pressed = true;
        if (ClickCursor != null)
            Cursor.SetCursor(ClickCursor, ClickHotspot, CursorMode.Auto);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_pressed && ClickCursor != null)
            Cursor.SetCursor(ClickCursor, ClickHotspot, CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        if (_hovering && HoverCursor != null)
            Cursor.SetCursor(HoverCursor, HoverHotspot, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _pressed = false;
        if (_hovering && HoverCursor != null)
            Cursor.SetCursor(HoverCursor, HoverHotspot, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnDisable()
    {
        if (_hovering || _pressed)
        {
            _hovering = false;
            _pressed = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
