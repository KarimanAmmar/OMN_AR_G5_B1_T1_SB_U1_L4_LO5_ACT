using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerWindow : EditorWindow
{
    [MenuItem("Oman Tools/VideoPlayer")]
    public static void ShowWindow()
    {
        GetWindow<VideoPlayerWindow>("Video Player");
    }

    private void OnGUI()
    {
        GUILayout.Label("Video Player", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Video Player Controller to Selected"))
        {
            AddController();
            AddVideoControls();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add VideoPlayerEvents to Selected"))
        {
            AddEvents();
        }
    }

    private static void AddController()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Video Player", "Please select a GameObject first.", "OK");
            return;
        }

        if (selected.GetComponent<VideoPlayerController>() != null)
        {
            EditorUtility.DisplayDialog("Video Player", "Selected object already has VideoPlayerController.", "OK");
            return;
        }

        if (selected.GetComponent<VideoPlayer>() == null)
        {
            EditorUtility.DisplayDialog("Video Player", "There's no VideoPlayer component on the selected game object.", "OK");
            return;
        }

        Undo.AddComponent<VideoPlayerController>(selected);
    }

    private static void AddEvents()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Video Player", "Please select a GameObject first.", "OK");
            return;
        }

        if (selected.GetComponent<VideoPlayer>() == null)
        {
            EditorUtility.DisplayDialog("Video Player", "Selected object does not have a VideoPlayer component.", "OK");
            return;
        }

        if (selected.GetComponent<VideoPlayerEvents>() != null)
        {
            EditorUtility.DisplayDialog("VideoPlayer", "Selected object already has VideoPlayerEvents.", "OK");
            return;
        }

        Undo.AddComponent<VideoPlayerEvents>(selected);
    }

    private static void AddVideoControls()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Video Player", "Please select a GameObject first.", "OK");
            return;
        }

        if (selected.GetComponent<VideoPlayer>() == null)
        {
            EditorUtility.DisplayDialog("Video Player", "Selected object does not have a VideoPlayer component.", "OK");
            return;
        }

        var prefab = Resources.Load<GameObject>("Prefabs/Video Controls");
        if (prefab == null)
        {
            EditorUtility.DisplayDialog(
                "Video Player",
                "Couldn't find prefab at Resources/Prefabs/Video Controls.prefab\n" +
                "Make sure it's located under a Resources folder and named exactly 'Video Controls'.",
                "OK");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null)
            instance = Instantiate(prefab);

        Undo.RegisterCreatedObjectUndo(instance, "Add Video Controls");
        Undo.SetTransformParent(instance.transform, selected.transform, "Parent Video Controls");
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        RectTransform rt = instance.GetComponent<RectTransform>();
        if (rt == null)
            rt = instance.GetComponentInChildren<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; // left / bottom
            rt.offsetMax = Vector2.zero; // right / top
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }

        Selection.activeGameObject = instance;
    }
}
