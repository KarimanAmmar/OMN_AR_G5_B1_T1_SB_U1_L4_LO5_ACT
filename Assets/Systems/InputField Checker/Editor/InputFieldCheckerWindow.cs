using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldCheckerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Input Field Checker")]
    public static void ShowWindow()
    {
        GetWindow<InputFieldCheckerWindow>("Input Field Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Input Field Checker Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Input Field Checker", GUILayout.Height(40)))
        {
            AddInputFieldChecker();
        }
    }

    private void AddInputFieldChecker()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        TMP_InputField inputField = selected.GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            EditorUtility.DisplayDialog("Error",
                "Selected GameObject does not have an InputField component.", "OK");
            return;
        }

        if (selected.GetComponent<InputFieldChecker>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "InputFieldChecker is already on the selected GameObject.", "OK");
            return;
        }

        EditorUtility.SetDirty(inputField);
        Undo.AddComponent<InputFieldChecker>(selected);
    }
}
