using UnityEditor;
using UnityEngine;

public class GameSequenceManagerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Game Sequence Manager")]
    public static void ShowWindow()
    {
        GetWindow<GameSequenceManagerWindow>("Game Sequence Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Sequence Manager Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add GameSequenceManager (and DependenciesManager) to Selected", GUILayout.Height(45)))
        {
            AddToSelected();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create GameSequenceManager in Scene", GUILayout.Height(30)))
        {
            CreateInScene();
        }
    }

    private void AddToSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Game Sequence Manager", "Please select a GameObject first.", "OK");
            return;
        }

        bool hasGameSequenceManager = selected.GetComponent<GameSequenceManager>() != null;
        bool hasDependenciesManager = selected.GetComponent<DependenciesManager>() != null;

        if (hasGameSequenceManager && hasDependenciesManager)
        {
            EditorUtility.DisplayDialog("Game Sequence Manager", "Selected object already has both managers.", "OK");
            return;
        }

        if (!hasDependenciesManager)
            Undo.AddComponent<DependenciesManager>(selected);

        if (!hasGameSequenceManager)
            Undo.AddComponent<GameSequenceManager>(selected);

        EditorUtility.SetDirty(selected);
    }

    private void CreateInScene()
    {
        GameSequenceManager existing = Object.FindFirstObjectByType<GameSequenceManager>();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorUtility.DisplayDialog("Game Sequence Manager", "A GameSequenceManager already exists in the scene.", "OK");
            return;
        }

        GameObject go = new GameObject("Game Sequence Manager");
        Undo.RegisterCreatedObjectUndo(go, "Create GameSequenceManager");
        go.AddComponent<DependenciesManager>();
        go.AddComponent<GameSequenceManager>();
        Selection.activeGameObject = go;
        EditorUtility.SetDirty(go);
    }
}

