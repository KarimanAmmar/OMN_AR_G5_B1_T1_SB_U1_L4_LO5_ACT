using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#region Shared Types
//──────────────────────────────────────────────────────────────────────────────
[Serializable]
public class SubQuestionHolder
{
    // ────────────── Inspector Groups ──────────────

    [TabGroup("Question", "Question Setup")]
    [LabelText("No. of Trials")]
    [MinValue(1)]
    public int trialsNo = 1;

    [TabGroup("Question", "Question Setup")]
    [HorizontalGroup("Question/Question Setup/Narration")]
    [LabelText("Play Narration?")]
    public bool playNarration;

    [TabGroup("Question", "Question Setup")]
    [HorizontalGroup("Question/Question Setup/Narration")]
    [ShowIf(nameof(playNarration))]
    [HideLabel]
    public AudioClip narrationId;

    [TabGroup("Question", "Question Setup")]
    [FoldoutGroup("Question/Question Setup/Dependencies")]
    [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, CustomRemoveElementFunction = nameof(RemoveDependency))]
    public List<string> dependencies;

    [TabGroup("Question", "Question Setup")]
    [FoldoutGroup("Question/Question Setup/Dependencies")]
    [Button]
    public void GenerateDependencies() => dependencies.ForEach(dependency => DependenciesManager.Instance.CreateDependency(dependency));

    private void RemoveDependency(string item)
    {
        DependenciesManager.Instance.RemoveDependency(item);
        dependencies.Remove(item);
    }

    [TabGroup("Question", "Question Setup")]
    [LabelText("Before Delay")]
    [MinValue(0f), Unit(Units.Second)]
    public float beforeDelay;

    [TabGroup("Question", "Question Hint")]
    [LabelText("Play Hint?")]
    public bool playHint;

    [TabGroup("Question", "Question Hint")]
    [ShowIf(nameof(playHint))]
    [HideLabel]
    public Animator QuestionHint;

    [TabGroup("Question", "Execution")]
    [LabelText("On Start")]
    public UnityEvent2 OnQuestionStart;

    [TabGroup("Question", "Execution")]
    [LabelText("On Wrong Attempt")]
    public UnityEvent2 OnWrongAttempt;

    [TabGroup("Question", "Execution")]
    [LabelText("On Wrong")]
    public UnityEvent2 OnWrong;

    [TabGroup("Question", "Execution")]
    [LabelText("On Success")]
    public UnityEvent2 OnSuccess;

    [TabGroup("Question", "Execution")]
    [LabelText("On End")]
    public UnityEvent2 OnQuestionEnd;

    [ReadOnly]
    public bool isCompleted;

    // ────────────── Helpers ──────────────
    internal bool IsReady()
    {
        foreach (var d in dependencies)
            if (!DependenciesManager.Instance.dependencies.Find(dep => dep.gO.name == d).dependency.IsCompleted()) return false;
        return true;
    }
}

[Serializable]
public class QuestionHolder
{
    [FoldoutGroup("Basic Info")]
    [LabelText("ID")]
    public string id;

    [FoldoutGroup("Sub-Questions")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    public List<SubQuestionHolder> subQuestions;
}
//──────────────────────────────────────────────────────────────────────────────
#endregion

public class GameSequenceManager : MonoBehaviour
{
    #region Singleton
    public static GameSequenceManager Instance;
    private void Awake() => Instance = Instance != null ? Instance : this;
    #endregion

    #region References
    [FoldoutGroup("References")] public AudioManager audioManager;
    [Space]
    [FoldoutGroup("References")] public UnityEngine.UI.Button hintButton;
    #endregion

    #region Questions Data
    [FoldoutGroup("Questions Data")]
    [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = true, ListElementLabelName = "id")]
    public List<QuestionHolder> questions;
    #endregion

    #region Search in Questions
    [FoldoutGroup("Search in Questions")]
    [ShowInInspector] private string searchId;

    [FoldoutGroup("Search in Questions")]
    [HideLabel]
    [ShowInInspector] private QuestionHolder selectedQuestionHolder;

#if UNITY_EDITOR
    //──────────────────── NEW NAVIGATION BUTTONS ────────────────────
    [HorizontalGroup("Get Buttons")]
    [Button("◀ Get Previous"), GUIColor(0.9f, 0.6f, 0.6f)]
    private void SelectPreviousQuestionHolder()
    {
        if (questions == null || questions.Count == 0) return;

        selectedQuestionHolder = selectedQuestionHolder == null
            ? questions[0]
            : questions[(questions.IndexOf(selectedQuestionHolder) - 1 + questions.Count) % questions.Count];

        searchId = selectedQuestionHolder.id;
        Debug.Log($"Selected previous QuestionHolder: {selectedQuestionHolder.id}");
    }

    [HorizontalGroup("Get Buttons", Order = 4)]
    [Button("Get Next ▶"), GUIColor(0.6f, 0.9f, 0.6f)]
    private void SelectNextQuestionHolderInspector()
    {
        if (questions == null || questions.Count == 0) return;

        selectedQuestionHolder = selectedQuestionHolder == null
            ? questions[0]
            : questions[(questions.IndexOf(selectedQuestionHolder) + 1) % questions.Count];

        searchId = selectedQuestionHolder.id;
        Debug.Log($"Selected next QuestionHolder: {selectedQuestionHolder.id}");
    }
#endif

    private void OnValidate()
    {
        Instance = Instance != null ? Instance : this;

        SearchQuestionHolderById();
    }

    private void SearchQuestionHolderById()
    {
        if (questions == null || questions.Count == 0)
        {
            selectedQuestionHolder = null;
            return;
        }

        selectedQuestionHolder = questions.FirstOrDefault(s => s.id == searchId);
    }
    #endregion

    #region Runtime Variables
    private QuestionHolder currentQuestionHolder;
    private SubQuestionHolder currentSubQuestionHolder;
    #endregion

    #region MonoBehaviours
    private void Start()
    {
        if (hintButton != null)
            hintButton.onClick.AddListener(PlayHint);
        else
            Debug.LogWarning("Hint button is not assigned.");

        SelectNextQuestion();
    }

    private void FixedUpdate()
    {
        if (currentSubQuestionHolder != null && currentSubQuestionHolder.IsReady())
        {
            bool lastSub = currentQuestionHolder.subQuestions.IndexOf(currentSubQuestionHolder)
                            == currentQuestionHolder.subQuestions.Count - 1;
            bool lastQuestion = questions.IndexOf(currentQuestionHolder) == questions.Count - 1;

            if (!(lastSub && lastQuestion)) SelectNextQuestion();
        }
    }
    #endregion

    #region Public Navigation
    [Button("Play Next ▶", ButtonSizes.Large, ButtonAlignment = 1, Stretch = false), GUIColor(0.9f, 0.9f, 0.6f), PropertyOrder(-2)]
    public void SelectNextQuestion()
    {
        if (currentQuestionHolder == null && currentSubQuestionHolder == null)
        {
            currentQuestionHolder = questions[0];

            if (currentQuestionHolder.subQuestions == null || currentQuestionHolder.subQuestions.Count == 0)
            {
                Debug.LogWarning($"Question {currentQuestionHolder.id} has no subQuestions.");
                return;
            }

            StartQuestion(currentQuestionHolder.subQuestions[0]);
        }
        else
        {
            if (currentQuestionHolder.subQuestions.IndexOf(currentSubQuestionHolder)
                < currentQuestionHolder.subQuestions.Count - 1)
            {
                StartQuestion(currentQuestionHolder.subQuestions[
                    currentQuestionHolder.subQuestions.IndexOf(currentSubQuestionHolder) + 1]);
            }
            else
            {
                int currentQuestionIndex = questions.IndexOf(currentQuestionHolder);
                if (currentQuestionIndex < questions.Count - 1)
                {
                    currentQuestionHolder = questions[currentQuestionIndex + 1];

                    if (currentQuestionHolder.subQuestions == null || currentQuestionHolder.subQuestions.Count == 0)
                    {
                        Debug.LogWarning($"Question {currentQuestionHolder.id} has no subQuestions.");
                        return;
                    }

                    StartQuestion(currentQuestionHolder.subQuestions[0]);
                }
                else
                {
                    Debug.LogWarning("No more questions.");
                }
            }
        }
    }

    [HorizontalGroup("GoToQuestion", width: 100)]
    [PropertyOrder(-1)]
    [LabelText("ID"), LabelWidth(20)]
    [ValueDropdown(nameof(GetAllTheQuestions))]
    public string targetQuestionId;

    private IEnumerable<string> GetAllTheQuestions() =>
        questions == null ? Enumerable.Empty<string>() : questions.Select(s => s.id);

    [HorizontalGroup("GoToQuestion")]
    [Button(ButtonSizes.Medium, ButtonAlignment = 1, Stretch = false), GUIColor(0.9f, 0.9f, 0.6f), PropertyOrder(-2)]
    public void GoToQuestion()
    {
        currentQuestionHolder = questions.Find(s => s.id == targetQuestionId);

        if (currentQuestionHolder != null)
        {
            if (currentQuestionHolder.subQuestions == null || currentQuestionHolder.subQuestions.Count == 0)
            {
                Debug.LogWarning($"Question {currentQuestionHolder.id} has no subQuestions.");
                return;
            }

            StartQuestion(currentQuestionHolder.subQuestions[0]);
        }
    }
    #endregion

    #region Question Execution
    private void StartQuestion(SubQuestionHolder subQuestion) =>
        StartCoroutine(StartQuestion_Coroutine(subQuestion));

    private IEnumerator StartQuestion_Coroutine(SubQuestionHolder subQuestion)
    {
        if (currentSubQuestionHolder != null)
        {
            currentSubQuestionHolder.OnQuestionEnd?.Invoke();
        }

        currentSubQuestionHolder = subQuestion;
        yield return new WaitForSeconds(currentSubQuestionHolder.beforeDelay);

        if (currentSubQuestionHolder.playNarration)
            audioManager.PlaySFX(currentSubQuestionHolder.narrationId);

        currentSubQuestionHolder.OnQuestionStart?.Invoke();
    }

    public void Wrong()
    {
        if (currentSubQuestionHolder != null)
        {
            currentSubQuestionHolder.OnWrongAttempt?.Invoke();

            currentSubQuestionHolder.trialsNo--;

            if (currentSubQuestionHolder.trialsNo == 0) DOVirtual.DelayedCall(2f, () =>
            {
                currentSubQuestionHolder.OnWrong?.Invoke();
                currentSubQuestionHolder.isCompleted = true;
                SelectNextQuestion();
            });
        }
    }

    public void Success()
    {
        if (currentSubQuestionHolder != null)
        {
            currentSubQuestionHolder.OnSuccess?.Invoke();
            currentSubQuestionHolder.isCompleted = true;
            SelectNextQuestion();
        }
    }
    #endregion

    #region Hint
    public void PlayHint()
    {
        if (!currentSubQuestionHolder.playHint) return;

        if (currentSubQuestionHolder.QuestionHint != null)
        {
            currentSubQuestionHolder.QuestionHint.enabled = true;
        }
    }
    #endregion

    #region Utility Queries
    public bool IsCurrentQuestion(QuestionHolder Question) => Question == currentQuestionHolder;

    public bool IsCurrentSubQuestion(SubQuestionHolder subQuestion) => subQuestion == currentSubQuestionHolder;
    #endregion
}