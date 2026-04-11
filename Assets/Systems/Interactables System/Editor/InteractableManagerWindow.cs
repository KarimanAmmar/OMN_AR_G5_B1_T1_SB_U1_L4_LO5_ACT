using UnityEditor;
using UnityEngine;

public class InteractableManagerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Interactable Manager")]
    public static void ShowWindow()
    {
        GetWindow<InteractableManagerWindow>("Interactable Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Interactable Manager Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Interactable Manager", GUILayout.Height(40)))
        {
            AddManager();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create Interactable Manager GameObject", GUILayout.Height(30)))
        {
            CreateManagerObject();
        }

        GUILayout.Space(15);
        GUILayout.Label("Interactable Component", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (GUILayout.Button("Add Interactable", GUILayout.Height(30)))
        {
            AddInteractable();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Scan Scene & Add Interactable To All", GUILayout.Height(30)))
        {
            ScanSceneAndAddInteractable();
        }
    }

    private void AddInteractable()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }
        if (selected.GetComponent<Interactable>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "Interactable is already on the selected GameObject.", "OK");
            return;
        }
        Undo.AddComponent<Interactable>(selected);
    }

    private void ScanSceneAndAddInteractable()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        int added = 0;
        int skipped = 0;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (var selectable in root.GetComponentsInChildren<UnityEngine.UI.Selectable>(true))
            {
                if (selectable.GetComponent<Interactable>() != null) { skipped++; continue; }
                Undo.AddComponent<Interactable>(selectable.gameObject);
                added++;
            }

            foreach (var draggable in root.GetComponentsInChildren<DraggableItem>(true))
            {
                if (draggable.GetComponent<Interactable>() != null) { skipped++; continue; }
                Undo.AddComponent<Interactable>(draggable.gameObject);
                added++;
            }

            foreach (var cursor in root.GetComponentsInChildren<CursorChanger>(true))
            {
                if (cursor.GetComponent<Interactable>() != null) { skipped++; continue; }
                Undo.AddComponent<Interactable>(cursor.gameObject);
                added++;
            }

            foreach (var hover in root.GetComponentsInChildren<UIHoverAction>(true))
            {
                if (hover.GetComponent<Interactable>() != null) { skipped++; continue; }
                Undo.AddComponent<Interactable>(hover.gameObject);
                added++;
            }
        }

        EditorUtility.DisplayDialog("Scan Complete",
            $"Added Interactable to {added} object(s).\n{skipped} already had it.", "OK");
    }

    private void AddManager()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }
        if (selected.GetComponent<InteractableManager>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "InteractableManager is already on the selected GameObject.", "OK");
            return;
        }
        Undo.AddComponent<InteractableManager>(selected);
    }

    private void CreateManagerObject()
    {
        InteractableManager existing = Object.FindFirstObjectByType<InteractableManager>();
        if (existing != null)
        {
            EditorUtility.DisplayDialog("Info",
                "An InteractableManager already exists in the scene.", "OK");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        GameObject go = new GameObject("Interactable Manager");
        Undo.RegisterCreatedObjectUndo(go, "Create InteractableManager");
        go.AddComponent<InteractableManager>();
        Selection.activeGameObject = go;
    }
}
