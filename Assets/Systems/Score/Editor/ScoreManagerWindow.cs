using UnityEditor;
using UnityEngine;

public class ScoreManagerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Score Manager")]
    public static void ShowWindow()
    {
        GetWindow<ScoreManagerWindow>("Score Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Score Manager Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add ScoreManager to Selected", GUILayout.Height(30)))
        {
            AddToSelected();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create ScoreManager in Scene", GUILayout.Height(30)))
        {
            CreateInScene();
        }
    }

    private void AddToSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Score Manager", "Please select a GameObject first.", "OK");
            return;
        }

        if (selected.GetComponent<ScoreManager>() != null)
        {
            EditorUtility.DisplayDialog("Score Manager", "Selected object already has a ScoreManager.", "OK");
            return;
        }

        Undo.AddComponent<ScoreManager>(selected);
    }

    private void CreateInScene()
    {
        ScoreManager existing = Object.FindFirstObjectByType<ScoreManager>();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorUtility.DisplayDialog("Score Manager", "A ScoreManager already exists in the scene.", "OK");
            return;
        }

        GameObject go = new GameObject("Score Manager");
        Undo.RegisterCreatedObjectUndo(go, "Create ScoreManager");
        go.AddComponent<ScoreManager>();
        Selection.activeGameObject = go;
        EditorUtility.SetDirty(go);
    }
}

