using System.Collections.Generic;
using Sirenix.OdinInspector;


/// <summary>
/// A single "answer group" – one or more synonyms that are all considered
/// the same correct answer.  Typing ANY synonym clears the whole group.
/// </summary>
[System.Serializable]
public class AnswerGroup
{
    [LabelText("Label")] public string label = "Answer Group";

    [ListDrawerSettings(ShowFoldout = true, DraggableItems = false)] [LabelText("Groups")]
    public List<string> synonyms = new List<string>();
}
