#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class SubQuestionHolderDrawer : OdinValueDrawer<SubQuestionHolder>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUI.backgroundColor = Color.white;

        var subStep = this.ValueEntry.SmartValue;
        if (GameSequenceManager.Instance != null && GameSequenceManager.Instance.IsCurrentSubQuestion(subStep))
        {
            GUI.backgroundColor = Color.yellow;
        }

        SirenixEditorGUI.DrawSolidRect(GUILayoutUtility.GetRect(0, 2), GUI.backgroundColor);

        CallNextDrawer(label);

        GUI.backgroundColor = Color.white;
    }
}
#endif
