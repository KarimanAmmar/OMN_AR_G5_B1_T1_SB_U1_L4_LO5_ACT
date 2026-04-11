using UnityEngine;
using System.Collections;
using TMPro;
using RTLTMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class WritePass : MonoBehaviour , IPointerClickHandler
{
    public TMP_InputField input;
    public RTLTextMeshPro setText;

    public GameObject writehere;
    void Start()
    {
        input.onValueChanged.AddListener(SetText);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetText(string value)
    {
        print(value);
        setText.text = value;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Set focus to the input field
        input.Select();
        input.ActivateInputField();
        writehere.SetActive(false);
    }

}
