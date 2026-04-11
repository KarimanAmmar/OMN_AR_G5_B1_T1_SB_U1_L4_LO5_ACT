using UnityEditor;
using UnityEngine;

public class CursorChangerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Cursor Changer")]
    public static void ShowWindow()
    {
        GetWindow<CursorChangerWindow>("Cursor Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cursor Changer Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Cursor Changer", GUILayout.Height(40)))
        {
            AddCursorChanger();
        }
    }

    private void AddCursorChanger()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<CursorChanger>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "CursorChanger is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<CursorChanger>(selected);
    }
}
