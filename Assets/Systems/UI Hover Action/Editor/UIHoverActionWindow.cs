using UnityEditor;
using UnityEngine;

public class UIHoverActionWindow : EditorWindow
{
    [MenuItem("Oman Tools/UI Hover Action")]
    public static void ShowWindow()
    {
        GetWindow<UIHoverActionWindow>("UI Hover Action");
    }

    private void OnGUI()
    {
        GUILayout.Label("UI Hover Action", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Adds UIHoverAction to the selected GameObject (On Hover / On Hover Exit).\n\n" +
            "Requirements:\n" +
            "- Must be under a Canvas (RectTransform).\n" +
            "- The object (or a child Graphic) must receive raycasts so the pointer can detect hover (e.g. Image with Raycast Target on).",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Add UI Hover Action", GUILayout.Height(40)))
        {
            AddUIHoverAction();
        }
    }

    private static void AddUIHoverAction()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("UI Hover Action", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<RectTransform>() == null)
        {
            EditorUtility.DisplayDialog(
                "UI Hover Action",
                "Selected GameObject must have a RectTransform (UI object under a Canvas).",
                "OK");
            return;
        }

        if (selected.GetComponent<UIHoverAction>() != null)
        {
            EditorUtility.DisplayDialog("UI Hover Action", "UIHoverAction is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<UIHoverAction>(selected);
    }
}
