using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AddSceneManagerToAllScenes
{
    [MenuItem("Oman Tools/Add Scene Manager")]
    public static void Execute()
    {
        if (Object.FindFirstObjectByType<SceneManager>())
        {
            EditorUtility.DisplayDialog("Warning","There's already a Scene Manager in this scene...", "Ok");
            return;
        }

        GameObject go = new GameObject("Scene Manager");
        go.AddComponent<SceneManager>();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());


        //string currentScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        //string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        //int added = 0;
        //int skipped = 0;

        //foreach (string guid in sceneGuids)
        //{
        //    string path = AssetDatabase.GUIDToAssetPath(guid);

        //    Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

        //    SceneManager existing = Object.FindFirstObjectByType<SceneManager>();
        //    if (existing != null)
        //    {
        //        skipped++;
        //        continue;
        //    }

        //    GameObject go = new GameObject("Scene Manager");
        //    go.AddComponent<SceneManager>();

        //    EditorSceneManager.MarkSceneDirty(scene);
        //    EditorSceneManager.SaveScene(scene);
        //    added++;
        //}

        //if (!string.IsNullOrEmpty(currentScenePath))
        //    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);

        //EditorUtility.DisplayDialog("Done",
        //    $"GameSceneManager added to {added} scene(s).\n{skipped} scene(s) already had it.", "OK");
    }
}
