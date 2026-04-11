using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TabSystemWindow : EditorWindow
{
    [MenuItem("Oman Tools/Tab System")]
    public static void ShowWindow()
    {
        GetWindow<TabSystemWindow>("Tab System");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tab System Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Tab System", GUILayout.Height(40)))
        {
            AddComponent<TabSystem>("TabSystem");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Tab Button", GUILayout.Height(30)))
        {
            AddTabButton();
        }
    }

    private void AddComponent<T>(string label) where T : Component
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }
        if (selected.GetComponent<T>() != null)
        {
            EditorUtility.DisplayDialog("Info", $"{label} is already on the selected GameObject.", "OK");
            return;
        }
        Undo.AddComponent<T>(selected);
    }

    private void AddTabButton()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }
        if (selected.GetComponent<Button>() == null)
        {
            EditorUtility.DisplayDialog("Error",
                "Selected GameObject does not have a Button component.", "OK");
            return;
        }
        if (selected.GetComponent<TabButton>() != null)
        {
            EditorUtility.DisplayDialog("Info", "TabButton is already on the selected GameObject.", "OK");
            return;
        }
        Undo.AddComponent<TabButton>(selected);
    }
}
