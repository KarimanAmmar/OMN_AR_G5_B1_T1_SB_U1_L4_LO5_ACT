using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    #region Singleton
    public static ScoreManager Instance;
    private void Awake() => Instance = Instance != null ? Instance : this;
    #endregion

    [Title("Quiz Setup")]
    [MinValue(1)]
    public int numberOfQuestions = 1;

    [MinValue(1)]
    public int wrongAttemptsPerQuestion = 1;

    [Title("Runtime")]
    [ReadOnly]
    public int score;

    [ReadOnly]
    public int currentQuestionNumber = 1;

    [ReadOnly]
    public int currentWrongAttempts;

    [ReadOnly]
    public int answeredQuestions;

    [Title("Answer Feedback")]
    [SerializeField] private Image emoji_happy;
    [SerializeField] private Image emoji_neutral;
    [SerializeField] private Image emoji_sad;
    [SerializeField] private Image emoji_cry;

    [SerializeField] private List<ScoreBarSlice> scorebar_slices;
    [SerializeField] private List<QuestionData> questions_data;


    [Title("Events")]
    [LabelText("On Question Started")]
    public UnityEvent2 onQuestionStarted;

    [LabelText("On Question Completed")]
    public UnityEvent2 onQuestionCompleted;

    [LabelText("On Correct Answer")]
    public UnityEvent2 onCorrectAnswer;

    [LabelText("On Wrong Attempt")]
    public UnityEvent2 onWrongAttempt;

    [LabelText("On Wrong Answer")]
    public UnityEvent2 onWrongAnswer;

    [LabelText("On All Answers Correct")]
    public UnityEvent2 onAllAnswersCorrect;

    [LabelText("On All Answers Wrong")]
    public UnityEvent2 onAllAnswersWrong;


    public bool IsQuizFinished => answeredQuestions >= numberOfQuestions;

    public void ResetScoreManager()
    {
        score = 0;
        currentQuestionNumber = 1;
        currentWrongAttempts = 0;
        answeredQuestions = 0;

        FireQuestionStarted();
    }

    public void CorrectAnswer()
    {
        if (IsQuizFinished) return;

        scorebar_slices[answeredQuestions].Slice_Correct.SetActive(true);

        ActivateEmojiForDuration(emoji_happy, 3f);

        score += 1;

        onCorrectAnswer?.Invoke();
        GetCurrentQuestionData()?.OnCorrect?.Invoke();
        CompleteCurrentQuestion();
    }

    public void WrongAttempt()
    {
        if (IsQuizFinished) return;

        if (currentWrongAttempts < wrongAttemptsPerQuestion)
            ActivateEmojiForDuration(emoji_sad, 3f);

        currentWrongAttempts += 1;
        onWrongAttempt?.Invoke();
        GetCurrentQuestionData()?.OnWrongAttempt?.Invoke();

        if (currentWrongAttempts >= wrongAttemptsPerQuestion)
        {
            scorebar_slices[answeredQuestions].Slice_Wrong.SetActive(true);

            ActivateEmojiForDuration(emoji_cry, 3f);

            onWrongAnswer?.Invoke();
            GetCurrentQuestionData()?.OnWrong?.Invoke();
            CompleteCurrentQuestion();
        }
    }

    private void CompleteCurrentQuestion()
    {
        FireQuestionCompleted();

        answeredQuestions += 1;
        currentWrongAttempts = 0;

        if (answeredQuestions >= numberOfQuestions)
        {
            if (score >= numberOfQuestions)
                onAllAnswersCorrect?.Invoke();
            else if (score == 0)
                onAllAnswersWrong?.Invoke();
        }

        if (!IsQuizFinished)
        {
            currentQuestionNumber += 1;
            FireQuestionStarted();
        }
    }

    public void SetWrongAttemptsForCurrentQuestion(int wrong_attempts)
    {
        wrongAttemptsPerQuestion = wrong_attempts;
    }

    public void FireQuestionStarted()
    {
        if (IsQuizFinished) return;
        onQuestionStarted?.Invoke();
        GetCurrentQuestionData()?.OnStarted?.Invoke();
    }

    public void FireQuestionCompleted()
    {
        if (IsQuizFinished) return;
        onQuestionCompleted?.Invoke();
        GetCurrentQuestionData()?.OnCompleted?.Invoke();
    }

    public QuestionData GetCurrentQuestionData()
    {
        return questions_data[answeredQuestions];
    }

    private void ActivateEmojiForDuration(Image emoji, float duration)
    {
        emoji.gameObject.SetActive(true);

        DOVirtual.DelayedCall(3.5f, () =>
        {
            emoji.gameObject.SetActive(false);
        });
    }

    private static void SetSliceActive(GameObject go, bool active)
    {
        go.SetActive(active);
    }


    public void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }





    [System.Serializable]
    public class QuestionData
    {
        public int question_number;

        [Space(10)]

        public UnityEvent2 OnStarted;
        public UnityEvent2 OnCompleted;
        public UnityEvent2 OnWrongAttempt;
        public UnityEvent2 OnWrong;
        public UnityEvent2 OnCorrect;
    }


    [System.Serializable]
    public class ScoreBarSlice
    {
        public GameObject Slice_Normal;
        public GameObject Slice_Correct;
        public GameObject Slice_Wrong;
    }
}