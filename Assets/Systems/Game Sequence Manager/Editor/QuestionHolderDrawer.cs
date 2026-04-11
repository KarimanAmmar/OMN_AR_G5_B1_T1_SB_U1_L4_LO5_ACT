#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class QuestionHolderDrawer : OdinValueDrawer<QuestionHolder>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUI.backgroundColor = Color.white;

        var step = this.ValueEntry.SmartValue;
        if (GameSequenceManager.Instance != null && GameSequenceManager.Instance.IsCurrentQuestion(step))
        {
            GUI.backgroundColor = Color.yellow;
        }

        SirenixEditorGUI.DrawSolidRect(GUILayoutUtility.GetRect(0, 2), GUI.backgroundColor);

        CallNextDrawer(label);

        GUI.backgroundColor = Color.white;
    }
}
#endif
