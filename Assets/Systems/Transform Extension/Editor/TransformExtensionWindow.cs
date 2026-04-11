using UnityEditor;
using UnityEngine;

public class TransformExtensionWindow : EditorWindow
{
    [MenuItem("Oman Tools/Transform Actions")]
    public static void ShowWindow()
    {
        GetWindow<TransformExtensionWindow>("Transform Actions");
    }

    private void OnGUI()
    {
        GUILayout.Label("Transform Actions Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Transform Actions", GUILayout.Height(40)))
        {
            AddTransformActions();
        }
    }

    private void AddTransformActions()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<TransformExtension>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "TransformActions is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<TransformExtension>(selected);
    }
}
