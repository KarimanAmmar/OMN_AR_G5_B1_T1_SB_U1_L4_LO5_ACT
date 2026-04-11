using System.Collections.Generic;
using UnityEngine;


public class Feedback : MonoBehaviour
{
    public int best_score = 5;

    [Space(10)]

    [SerializeField] private List<ScoreBarSlice> score_bar_slices;

    [Space(10)]

    [Header("Excellent")]
    public List<FeedbackUISound> excellent;

    //[SerializeField] private List<AudioClip> excellent_sounds;
    //[SerializeField] private List<GameObject> excellent_ui_words;

    [Header("Medium")]
    public List<FeedbackUISound> medium;

    //[SerializeField] private List<AudioClip> medium_sounds;
    //[SerializeField] private List<GameObject> medium_ui_words;

    [Header("Loser")]
    public List<FeedbackUISound> loser;

    //[SerializeField] private List<AudioClip> loser_sounds;
    //[SerializeField] private List<GameObject> loser_ui_words;



    private int score;
    private float score_precent;


    private void Start()
    {
        score = PlayerPrefs.GetInt("Score", 0);
        score_precent = ((float)score / (float)best_score) * 100;

        ShowUIAndPlaySound();
    }


    private void ShowUIAndPlaySound()
    {
        for (int i = 0; i< best_score;i++)
        {
            if (i < score)
            {
                score_bar_slices[i].Slice_Correct.SetActive(true);

                continue;
            }

            score_bar_slices[i].Slice_Wrong.SetActive(true);
        }

        if (score_precent >= 80)
        {
            int rnd = Random.Range(0, excellent.Count);

            excellent[rnd].Ui.SetActive(true);
            AudioManager.Instance.PlaySFX(excellent[rnd].Sound);
        }
        else if (score_precent > 40 && score_precent < 80)
        {
            int rnd = Random.Range(0, medium.Count);

            medium[rnd].Ui.SetActive(true);
            AudioManager.Instance.PlaySFX(medium[rnd].Sound);
        }
        else
        {
            int rnd = Random.Range(0, loser.Count);

            loser[rnd].Ui.SetActive(true);
            AudioManager.Instance.PlaySFX(loser[rnd].Sound);
        }
    }










    [System.Serializable]
    public class ScoreBarSlice
    {
        public GameObject Slice_Normal;
        public GameObject Slice_Correct;
        public GameObject Slice_Wrong;
    }


    [System.Serializable]
    public class FeedbackUISound
    {
        public GameObject Ui;
        public AudioClip Sound;
    }
}