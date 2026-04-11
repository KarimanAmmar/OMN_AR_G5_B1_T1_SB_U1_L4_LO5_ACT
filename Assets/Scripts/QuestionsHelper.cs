using UnityEngine;
using UnityEngine.UI;

public class QuestionsHelper : MonoBehaviour
{
    [SerializeField] Button2[] buttonsList;
    [SerializeField] GameObject[] Wrongoutlines;
    [SerializeField] Image[] images;
    public void whenCorrectButtonClicked()
    {
        if (Wrongoutlines.Length > 0)
        {
            for (int i = 0; i < Wrongoutlines.Length; i++)
            {
                Wrongoutlines[i].SetActive(false);
            }
        }
        if (buttonsList != null)
        {
            for (int i = 0; i < buttonsList.Length; i++)
            {
                buttonsList[i].interactable = false;
            }
        }
        DisableImageRaycast();
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
    public void DisableImageRaycast()
    {
        for(int i = 0;i < images.Length; i++)
        {
            images[i].raycastTarget = false;
        }
    }
    public void EnableImageRaycast()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].raycastTarget = true;
        }
    }
}
