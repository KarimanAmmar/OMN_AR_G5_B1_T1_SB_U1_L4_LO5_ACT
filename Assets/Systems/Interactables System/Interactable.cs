using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public string Group = "";

    [SerializeField] private bool ignore;
    [SerializeField] private bool interactable = true;

    private Selectable selectable;
    private DraggableItem draggable;
    private UIHoverAction hover;

    private InteractableType interactableType;

    public bool IsInteractable => interactable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        draggable = GetComponent<DraggableItem>();
        hover = GetComponent<UIHoverAction>();


        if (selectable)
            interactableType = InteractableType.UnitySelectable;
        else if (draggable)
            interactableType = InteractableType.DraggableItem;
        else if (hover)
            interactableType = InteractableType.Hoverable;

        Apply();
    }

    public void SetInteractable(bool interactable)
    {
        if (ignore)
            return;

        this.interactable = interactable;
        Apply();
    }

    private void Apply()
    {
        if (interactableType == InteractableType.UnitySelectable && selectable != null)
        {
            selectable.interactable = interactable;
        }
    }


    public enum InteractableType
    {
        UnitySelectable, DraggableItem, Hoverable
    }
}