using UnityEditor;
using UnityEngine;

public class DragAndDropWindow : EditorWindow
{
    [MenuItem("Oman Tools/Drag && Drop")]
    public static void ShowWindow()
    {
        GetWindow<DragAndDropWindow>("Drag & Drop");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag & Drop Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Draggable Item", GUILayout.Height(40)))
        {
            AddDraggableItem();
        }
    }

    private void AddDraggableItem()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<RectTransform>() == null)
        {
            EditorUtility.DisplayDialog("Error",
                "Selected GameObject does not have a RectTransform.\nIt must be a UI / Canvas element.", "OK");
            return;
        }

        if (selected.GetComponent<DraggableItem>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "DraggableItem is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<DraggableItem>(selected);
    }
}
