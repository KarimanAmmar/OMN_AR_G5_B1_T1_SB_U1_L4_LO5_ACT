using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabSystem : MonoBehaviour
{
    [Header("Tabs")]
    public List<TabButton> Tabs = new();

    [Header("Options")]
    [Tooltip("Index of the tab to show on Start (-1 = none)")]
    public int DefaultTabIndex;

    [Header("Events")]
    public UnityEvent<int> OnTabChanged;

    private int _activeIndex = -1;

    public int ActiveIndex => _activeIndex;

    private void Start()
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            if (Tabs[i] != null)
                Tabs[i].Initialize(this, i);
        }

        if (DefaultTabIndex >= 0 && DefaultTabIndex < Tabs.Count)
            SelectTab(DefaultTabIndex);
        else
            DeactivateAll();
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= Tabs.Count) return;

        for (int i = 0; i < Tabs.Count; i++)
        {
            if (Tabs[i] != null)
                Tabs[i].SetActive(i == index);
        }

        _activeIndex = index;
        OnTabChanged?.Invoke(index);
    }

    public void SelectTab(TabButton tab)
    {
        int index = Tabs.IndexOf(tab);
        if (index >= 0)
            SelectTab(index);
    }

    public void DeactivateAll()
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            if (Tabs[i] != null)
                Tabs[i].SetActive(false);
        }
        _activeIndex = -1;
    }

    public void SelectNext()
    {
        if (Tabs.Count == 0) return;
        SelectTab((_activeIndex + 1) % Tabs.Count);
    }

    public void SelectPrevious()
    {
        if (Tabs.Count == 0) return;
        SelectTab((_activeIndex - 1 + Tabs.Count) % Tabs.Count);
    }
}
