using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DependencyHolder
{
    public GameObject gO;
    public SimpleDependency dependency;
}

public class DependenciesManager : MonoBehaviour
{
    public static DependenciesManager Instance;

    [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false)]
    public List<DependencyHolder> dependencies;

    private void OnValidate() => Instance = Instance != null ? Instance : this;

    private void Awake() => Instance = Instance != null ? Instance : this;

    public void CreateDependency(string name)
    {
        if (String.IsNullOrEmpty(name)) return;

        if (dependencies.Find(d => d.gO.name == name) != null) return;

        GameObject newDependency = new GameObject(name);
        newDependency.transform.SetParent(transform);
        dependencies.Add(new DependencyHolder
        {
            dependency = newDependency.AddComponent<SimpleDependency>(),
            gO = newDependency
        });
    }

    public void RemoveDependency(string name)
    {
        if (String.IsNullOrEmpty(name)) return;

        if (dependencies.Find(d => d.gO.name == name) == null) return;

        DependencyHolder target = dependencies.Find(d => d.gO.name == name);
        DestroyImmediate(target.gO);
        dependencies.Remove(target);
    }

    [StringDropdown(nameof(GetAllDependencies))]
    public void CompleteDependency(string name)
    {
        dependencies.Find(d => d.gO.name == name).dependency._Complete();
    }

    private IEnumerable<string> GetAllDependencies() => dependencies.Select(d => d.gO.name);
}
