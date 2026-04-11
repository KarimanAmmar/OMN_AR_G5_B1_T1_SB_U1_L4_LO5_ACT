using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent2 onClick;
    private BoxCollider2D[] boxColliders;
    private bool hasBeenClicked = false;

    private void Awake()
    {
        // Cache all BoxCollider2D components on this GameObject
        boxColliders = GetComponents<BoxCollider2D>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasBeenClicked)
            return;

        hasBeenClicked = true;

        // Fire the event
        onClick?.Invoke();

        // Disable all BoxCollider2D components
        foreach (BoxCollider2D collider in boxColliders)
        {
            collider.enabled = false;
        }
    }
    public void FireOnClick()
    {
        onClick?.Invoke();
    }
}