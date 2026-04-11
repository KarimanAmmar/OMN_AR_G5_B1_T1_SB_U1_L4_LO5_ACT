using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }

    //private readonly List<Interactable> _interactables = new();
    private List<Interactable> _interactables = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ScanAll();
    }

    public void ScanAll()
    {
        _interactables.Clear();

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (var interactable in root.GetComponentsInChildren<Interactable>(true))
                _interactables.Add(interactable);
        }
    }

    public void Register(Interactable interactable)
    {
        if (!_interactables.Contains(interactable))
            _interactables.Add(interactable);
    }

    public void Unregister(Interactable interactable)
    {
        _interactables.Remove(interactable);
    }



    // ─── Enable / Disable All ───

    public void EnableAll()
    {
        foreach (var i in _interactables)
            i.SetInteractable(true);
    }

    public void DisableAll()
    {
        foreach (var i in _interactables)
            i.SetInteractable(false);
    }

    // ─── By Group ───

    public void EnableGroup(string group)
    {
        foreach (var i in _interactables)
        {
            if (i.Group == group)
                i.SetInteractable(true);
        }
    }

    public void DisableGroup(string group)
    {
        foreach (var i in _interactables)
        {
            if (i.Group == group)
                i.SetInteractable(false);
        }
    }

    // ─── Selective ───

    public void EnableOnly(string group)
    {
        foreach (var i in _interactables)
        {
            if (i.Group == group)
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    public void EnableOnly(Interactable single)
    {
        foreach (var i in _interactables)
        {
            if (i == single)
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    public void DisableAllExcept(string group)
    {
        foreach (var i in _interactables)
        {
            if (i.Group == group)
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    public void DisableAllExcept(Interactable single)
    {
        foreach (var i in _interactables)
        {
            if (i == single)
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    // ─── By List ───

    public void DisableAllExcept(List<Interactable> exceptions)
    {
        var set = new HashSet<Interactable>(exceptions);
        foreach (var i in _interactables)
        {
            if (set.Contains(i))
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    public void EnableOnly(List<Interactable> list)
    {
        var set = new HashSet<Interactable>(list);
        foreach (var i in _interactables)
        {
            if (set.Contains(i))
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    public void EnableList(List<Interactable> list)
    {
        foreach (var i in list)
        {
            if (i != null)
                i.SetInteractable(true);
        }
    }

    public void DisableList(List<Interactable> list)
    {
        foreach (var i in list)
        {
            if (i != null)
                i.SetInteractable(false);
        }
    }

    // ─── Multiple Groups ───

    public void EnableGroups(params string[] groups)
    {
        var set = new HashSet<string>(groups);
        foreach (var i in _interactables)
        {
            if (set.Contains(i.Group))
                i.SetInteractable(true);
        }
    }

    public void DisableGroups(params string[] groups)
    {
        var set = new HashSet<string>(groups);
        foreach (var i in _interactables)
        {
            if (set.Contains(i.Group))
                i.SetInteractable(false);
        }
    }

    public void EnableOnlyGroups(params string[] groups)
    {
        var set = new HashSet<string>(groups);
        foreach (var i in _interactables)
        {
            if (set.Contains(i.Group))
                i.SetInteractable(true);
            else
                i.SetInteractable(false);
        }
    }

    // ─── Query ───

    public List<Interactable> GetAll() => new(_interactables);

    public List<Interactable> GetGroup(string group)
    {
        var result = new List<Interactable>();
        foreach (var i in _interactables)
        {
            if (i.Group == group)
                result.Add(i);
        }
        return result;
    }

    public int Count => _interactables.Count;
}
