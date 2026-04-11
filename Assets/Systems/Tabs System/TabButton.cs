using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabButton : MonoBehaviour
{
    [Header("Content")]
    [Tooltip("The GameObject to show/hide when this tab is selected/deselected")]
    public GameObject Content;

    [Header("Visual Feedback (optional)")]
    public Color ActiveColor = Color.white;
    public Color InactiveColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Image TabImage;

    [Header("Events")]
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    private TabSystem _system;
    private int _index;
    private Interactable _interactable;

    private bool CanInteract => _interactable == null || _interactable.IsInteractable;

    public void Initialize(TabSystem system, int index)
    {
        _system = system;
        _index = index;
        _interactable = GetComponent<Interactable>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (!CanInteract) return;
        if (_system != null)
            _system.SelectTab(_index);
    }

    public void SetActive(bool active)
    {
        if (Content != null)
            Content.SetActive(active);

        if (TabImage != null)
            TabImage.color = active ? ActiveColor : InactiveColor;

        if (active)
            OnSelected?.Invoke();
        else
            OnDeselected?.Invoke();
    }
}
