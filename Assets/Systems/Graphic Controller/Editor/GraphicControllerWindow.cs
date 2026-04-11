using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GraphicControllerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Graphic Controller")]
    public static void ShowWindow()
    {
        GetWindow<GraphicControllerWindow>("Graphic Controller");
    }

    private void OnGUI()
    {
        GUILayout.Label("Graphic Controller Tool", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Add Graphic Controller", GUILayout.Height(40)))
        {
            AddGraphicController();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Add CanvasGroup (optional)", GUILayout.Height(25)))
        {
            AddCanvasGroup();
        }
    }

    private static void AddGraphicController()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Graphic Controller", "Please select a GameObject.", "OK");
            return;
        }

        Graphic g = selected.GetComponent<Graphic>();
        if (g == null)
        {
            EditorUtility.DisplayDialog(
                "Graphic Controller",
                "Selected GameObject does not have a Graphic component (Image/Text/RawImage/etc).",
                "OK");
            return;
        }

        if (selected.GetComponent<GraphicController>() != null)
        {
            EditorUtility.DisplayDialog("Graphic Controller", "GraphicController is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<GraphicController>(selected);
    }

    private static void AddCanvasGroup()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Graphic Controller", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<CanvasGroup>() != null)
        {
            EditorUtility.DisplayDialog("Graphic Controller", "CanvasGroup is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<CanvasGroup>(selected);
    }
}

