using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillerWindow : EditorWindow
{
    [MenuItem("Oman Tools/Image Filler")]
    public static void ShowWindow()
    {
        GetWindow<ImageFillerWindow>("Image Filler");
    }

    private void OnGUI()
    {
        GUILayout.Label("Image Filler Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Add Image Filler", GUILayout.Height(40)))
        {
            AddImageFiller();
        }
    }

    private void AddImageFiller()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        Image image = selected.GetComponent<Image>();
        if (image == null)
        {
            EditorUtility.DisplayDialog("Error",
                "Selected GameObject does not have an Image component.", "OK");
            return;
        }

        if (selected.GetComponent<ImageFiller>() != null)
        {
            EditorUtility.DisplayDialog("Info",
                "ImageFiller is already on the selected GameObject.", "OK");
            return;
        }

        Undo.RecordObject(image, "Set Image Type to Filled");
        image.type = Image.Type.Filled;
        image.fillAmount = 0f;
        EditorUtility.SetDirty(image);

        Undo.AddComponent<ImageFiller>(selected);
    }
}
