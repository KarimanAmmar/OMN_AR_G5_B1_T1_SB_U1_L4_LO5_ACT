using UnityEngine;
using UnityEditor;

public class SceneFaderWindow : EditorWindow
{
    [MenuItem("Oman Tools/Scene Fader")]
    public static void ShowWindow()
    {
        GetWindow<SceneFaderWindow>("Scene Fader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Fader Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Scene Fader to Selected"))
        {
            AddToSelected();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create Scene Fader in Scene"))
        {
            CreateInScene();
        }

        GUILayout.Space(15);
        GUILayout.Label("Info", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "SceneFader creates its own overlay Canvas at runtime.\n\n" +
            "Use SceneFader.Instance.FadeIn() / FadeOut() from code,\n" +
            "or use SceneManager's WithFade methods:\n" +
            "  - LoadSceneWithFade(sceneName)\n",
            MessageType.Info);
    }

    private void AddToSelected()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Scene Fader", "Please select a GameObject first.", "OK");
            return;
        }

        if (selected.GetComponent<SceneFader>() != null)
        {
            EditorUtility.DisplayDialog("Scene Fader", "Selected object already has a SceneFader.", "OK");
            return;
        }

        Undo.AddComponent<SceneFader>(selected);
    }

    private void CreateInScene()
    {
        if (FindFirstObjectByType<SceneFader>() != null)
        {
            EditorUtility.DisplayDialog("Scene Fader", "A SceneFader already exists in the scene.", "OK");
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Scene Fader",
                "No Canvas found in the scene. Please add a Canvas first.",
                "OK");
            return;
        }

        var go = new GameObject("Scene Fader");
        Undo.RegisterCreatedObjectUndo(go, "Create Scene Fader");
        Undo.SetTransformParent(go.transform, canvas.transform, "Parent Scene Fader to Canvas");
        go.transform.SetAsLastSibling();
        Undo.AddComponent<SceneFader>(go);
        Selection.activeGameObject = go;
    }
}
