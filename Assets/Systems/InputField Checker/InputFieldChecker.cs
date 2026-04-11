using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;


[RequireComponent(typeof(TMP_InputField))]
public class InputFieldChecker : MonoBehaviour
{
    private TMP_InputField inputField;

    [SerializeField] private Button2 button_confirm;


    [SerializeField] private bool use_character_limit;
    [ShowIf("use_character_limit"), SerializeField] private int min_character_limit;

    [Space(10)]

    // ── Answer groups ────────────────────────────────────────────────────────
    [Tooltip("Each group is a set of synonyms for the same answer.\n" +
             "Typing ANY synonym inside a group counts as answering that group correctly.\n" +
             "In group mode each group is tracked independently — answering one removes only that group.")]
    [ListDrawerSettings(ShowFoldout = true, DraggableItems = false, ListElementLabelName = "label")]
    [SerializeField]
    private List<AnswerGroup> answerGroups = new List<AnswerGroup>();

    // ── Group mode ──────────────────────────────────────────────────────────
    [SerializeField] private bool useGroup = false;

    // ── Broadcast sync ───────────────────────────────────────────────────────
    [ShowIf("@useGroup == true")]
    [Tooltip("When enabled, answering a group broadcasts to ALL other InputFieldCheckers\n" +
             "so they remove the same group automatically — no cross-references needed.")]
    [SerializeField] private bool broadcastToOtherCheckers = true;

    // ── Events ──────────────────────────────────────────────────────────────
    [FoldoutGroup("Events")] [SerializeField] private UnityEvent2 OnValueCorrect;
    [FoldoutGroup("Events")] [SerializeField] private UnityEvent2 OnValueWrong;

    [FoldoutGroup("Events")]
    [ShowIf("@useGroup == true")]
    [SerializeField] private UnityEvent2 OnItemRemoved;

    [FoldoutGroup("Events")]
    [ShowIf("@useGroup == true")]
    [SerializeField] private UnityEvent2 OnListEmpty;

    // ── Static broadcast ─────────────────────────────────────────────────────
    // Broadcasts the ORIGINAL index (position in the Inspector list) of the
    // answered group so every checker can remove by stable position — no
    // string-matching across instances needed.
    private static event Action<int, InputFieldChecker> GroupAnsweredBroadcast;

    // Parallel list to answerGroups that remembers each entry's original index.
    private List<int> originalIndices = new List<int>();

    // ────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        // Map every group to its original slot (0, 1, 2 … N-1).
        originalIndices.Clear();
        for (int i = 0; i < answerGroups.Count; i++)
            originalIndices.Add(i);

        if (use_character_limit && button_confirm)
        {
            button_confirm.interactable = false;
        }

        inputField.onValueChanged.AddListener((s) => CheckCharacterLimit());
    }

    private void CheckCharacterLimit()
    {
        if (button_confirm)
            button_confirm.interactable = inputField.text.Length >= min_character_limit;
    }

    private void OnEnable() => GroupAnsweredBroadcast += OnRemoteGroupAnswered;
    private void OnDisable() => GroupAnsweredBroadcast -= OnRemoteGroupAnswered;

    private void Start()
    {
        button_confirm?.onClick.AddListener(CheckValue);
    }

    // ────────────────────────────────────────────────────────────────────────
    public void CheckValue()
    {
        string typed = inputField.text.Trim();
        if (useGroup) CheckGroupMode(typed);
        else CheckSingleMode(typed);


        string normalized = inputField.text.Normalize(System.Text.NormalizationForm.FormD);
    }

    private void CheckGroupMode(string typed)
    {

        // ── Local + broadcast ────────────────────────────────────────────────
        if (TryRemoveGroup(typed, out int originalIndex))
        {

            OnValueCorrect?.Invoke();
            OnItemRemoved?.Invoke();
            if (answerGroups.Count == 0) OnListEmpty?.Invoke();

            // Broadcast the stable original index — not the string.
            if (broadcastToOtherCheckers)
                GroupAnsweredBroadcast?.Invoke(originalIndex, this);
        }
        else
        {
            OnValueWrong?.Invoke();
        }
    }

    /// <summary>
    /// Called on every OTHER checker when one checker answers a group.
    /// Removes by original index so shifting lists never cause mismatches.
    /// </summary>
    private void OnRemoteGroupAnswered(int originalIndex, InputFieldChecker source)
    {
        if (source == this || !useGroup) return;

        int slot = originalIndices.IndexOf(originalIndex);
        if (slot < 0) return;   // already removed on this checker

        originalIndices.RemoveAt(slot);
        answerGroups.RemoveAt(slot);


        OnItemRemoved?.Invoke();
        if (answerGroups.Count == 0) OnListEmpty?.Invoke();
    }

    /// <summary>
    /// Finds the group containing <paramref name="synonym"/>, removes it from
    /// both answerGroups and originalIndices, and returns the original index.
    /// </summary>
    private bool TryRemoveGroup(string synonym, out int originalIndex)
    {
        for (int i = 0; i < answerGroups.Count; i++)
        {
            if (answerGroups[i].synonyms.Contains(synonym))
            {
                originalIndex = originalIndices[i];
                answerGroups.RemoveAt(i);
                originalIndices.RemoveAt(i);
                return true;
            }
        }
        originalIndex = -1;
        return false;
    }

    private void CheckSingleMode(string typed)
    {
        foreach (AnswerGroup group in answerGroups)
        {
            if (group.synonyms.Contains(typed)) { OnValueCorrect?.Invoke(); return; }
        }
        OnValueWrong?.Invoke();
    }


#if UNITY_EDITOR
    public IReadOnlyList<AnswerGroup> AnswerGroups => answerGroups;
#endif
}