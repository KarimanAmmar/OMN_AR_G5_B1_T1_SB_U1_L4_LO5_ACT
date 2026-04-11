using UnityEngine;
using UnityEngine.UI;

public class QuestionsHelper : MonoBehaviour
{
    [SerializeField] Button2[] buttonsList;
    [SerializeField] GameObject[] Wrongoutlines;
    public void whenCorrectButtonClicked()
    {
        for (int i = 0; i < Wrongoutlines.Length; i++)
        {
            Wrongoutlines[i].SetActive(false);
        }
        for (int i = 0;i < buttonsList.Length;i++) 
        {
            buttonsList[i].interactable = false;
        }
    }
    public void EnableQuestions()
    {
        for (int i = 0; i < buttonsList.Length; i++)
        {
            buttonsList[i].interactable = true;
        }
    }
    public void DisableQuestions()
    {
        for (int i = 0; i < buttonsList.Length; i++)
        {
            buttonsList[i].interactable = false;
        }
    }

}
