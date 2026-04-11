using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NumpadWindow : EditorWindow
{
    [MenuItem("Oman Tools/Numpad")]
    public static void ShowWindow()
    {
        GetWindow<NumpadWindow>("Numpad");
    }

    private void OnGUI()
    {
        GUILayout.Label("Numpad Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Numpad", GUILayout.Height(40)))
        {
            AddNumpadComponent();
        }

        GUILayout.Space(15);
        GUILayout.Label("Add Numpad Action (requires Button)", EditorStyles.boldLabel);
        GUILayout.Space(5);

        for (int row = 0; row < 3; row++)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < 3; col++)
            {
                int digit = row * 3 + col + 1;
                if (GUILayout.Button(digit.ToString(), GUILayout.Height(30)))
                {
                    AddNumpadAction((NumpadActionType)digit);
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("0", GUILayout.Height(30)))
        {
            AddNumpadAction(NumpadActionType.Digit0);
        }
        if (GUILayout.Button("Delete", GUILayout.Height(30)))
        {
            AddNumpadAction(NumpadActionType.Delete);
        }
        GUILayout.EndHorizontal();
    }

    private void AddNumpadComponent()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        if (selected.GetComponent<Numpad>() != null)
        {
            EditorUtility.DisplayDialog("Info", "Numpad is already on the selected GameObject.", "OK");
            return;
        }

        Undo.AddComponent<Numpad>(selected);
    }

    private void AddNumpadAction(NumpadActionType actionType)
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

        NumpadAction existing = selected.GetComponent<NumpadAction>();
        if (existing != null)
        {
            existing.ActionType = actionType;
            EditorUtility.SetDirty(existing);
            EditorUtility.DisplayDialog("Info",
                $"NumpadAction already existed — updated action to {actionType}.", "OK");
            return;
        }

        NumpadAction action = Undo.AddComponent<NumpadAction>(selected);
        action.ActionType = actionType;
    }
}
