using UnityEditor;
using UnityEngine;

public class AudioManagerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Audio Manager")]
    public static void ShowWindow()
    {
        GetWindow<AudioManagerWindow>("Audio Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Audio Manager Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Audio Manager", GUILayout.Height(40)))
        {
            AddAudioManager();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Audio Manager GameObject", GUILayout.Height(30)))
        {
            CreateAudioManagerObject();
        }
    }

    private void AddAudioManager()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<AudioManager>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "AudioManager is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<AudioManager>(selected);
    }

    private void CreateAudioManagerObject()
    {
        AudioManager existing = Object.FindFirstObjectByType<AudioManager>();
        if (existing != null)
        {
            EditorUtility.DisplayDialog("Info",
                "An AudioManager already exists in the scene.", "OK");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        GameObject go = new GameObject("Audio Manager");
        Undo.RegisterCreatedObjectUndo(go, "Create AudioManager");
        go.AddComponent<AudioManager>();
        Selection.activeGameObject = go;
    }
}
