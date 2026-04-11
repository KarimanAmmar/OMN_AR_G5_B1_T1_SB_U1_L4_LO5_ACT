using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Sirenix.OdinInspector;


/// <summary>
/// Manages a shared list of pending AnswerGroups collected automatically
/// from all InputFieldCheckers that reference this component.
///
/// You do NOT need to fill in any list here manually — each InputFieldChecker
/// registers its own answer groups in Awake.  OnListEmpty fires when every
/// group across all registered checkers has been answered.
/// </summary>
public class InputFieldCheckerGroup : MonoBehaviour
{
    // ── UI (optional) ────────────────────────────────────────────────────────
    [Tooltip("Optional: a parent Transform whose children are the list-item UI elements.\nChild 0 → first registered group, Child 1 → second, etc.\nLeave empty if you handle the display yourself.")]
    [SerializeField] private Transform listContainer;

    // ── Events ───────────────────────────────────────────────────────────────
    [Tooltip("Fired every time a group is removed from the list.")]
    [SerializeField] private UnityEvent OnItemRemoved;

    [Tooltip("Fired when the last group has been removed (all groups answered).")]
    [SerializeField] private UnityEvent OnListEmpty;


    // ── Runtime state ────────────────────────────────────────────────────────
    // Flat list of all groups collected from every registered InputFieldChecker.
    private List<AnswerGroup> allGroups = new List<AnswerGroup>();

    // Indices into allGroups that are still pending (not yet answered).
    private List<int> runtimeIndices;


    /// <summary>
    /// Called by InputFieldChecker in Awake to hand over its answer groups.
    /// All groups from all checkers are collected before Start runs.
    /// </summary>
    public void RegisterGroups(List<AnswerGroup> groups)
    {
        allGroups.AddRange(groups);
    }


    private void Start()
    {
        // Build the pending-index list from everything that was registered.
        runtimeIndices = new List<int>();
        for (int i = 0; i < allGroups.Count; i++)
            runtimeIndices.Add(i);

        if (listContainer != null)
            RefreshListUI();
    }


    /// <summary>
    /// Called by InputFieldChecker when a correct synonym is entered.
    /// Finds the group that contains <paramref name="synonym"/> and removes
    /// that entire group from the pending list.
    /// </summary>
    public void RemoveGroup(string synonym)
    {
        if (runtimeIndices == null) return;

        for (int r = 0; r < runtimeIndices.Count; r++)
        {
            int groupIndex = runtimeIndices[r];
            AnswerGroup group = allGroups[groupIndex];

            if (group.synonyms.Contains(synonym))
            {
                runtimeIndices.RemoveAt(r);

                if (listContainer != null)
                    HideUIChild(groupIndex);

                OnItemRemoved?.Invoke();

                if (runtimeIndices.Count == 0)
                    OnListEmpty?.Invoke();

                return;
            }
        }
    }


    // ── UI helpers ───────────────────────────────────────────────────────────

    /// <summary>Activates/deactivates children to match the pending indices.</summary>
    private void RefreshListUI()
    {
        int childCount = listContainer.childCount;
        for (int i = 0; i < childCount; i++)
        {
            bool pending = runtimeIndices.Contains(i);
            listContainer.GetChild(i).gameObject.SetActive(pending);
        }
    }

    /// <summary>Hides the UI child at position <paramref name="childIndex"/>.</summary>
    private void HideUIChild(int childIndex)
    {
        if (childIndex < listContainer.childCount)
            listContainer.GetChild(childIndex).gameObject.SetActive(false);
    }


    // ── Editor helper ────────────────────────────────────────────────────────
#if UNITY_EDITOR
    /// <summary>Returns the indices of groups still pending (read-only, for debugging).</summary>
    public IReadOnlyList<int> RuntimeIndices => runtimeIndices;

    /// <summary>Returns all registered groups (read-only, for debugging).</summary>
    public IReadOnlyList<AnswerGroup> AllGroups => allGroups;
#endif
}
