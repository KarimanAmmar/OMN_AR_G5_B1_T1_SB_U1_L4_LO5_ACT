using UnityEngine;
using TMPro;

public enum NumpadTargetType
{
    InputField,
    TextMeshProUGUI
}

public class Numpad : MonoBehaviour
{
    public NumpadTargetType TargetType;

    [Header("Assign ONE of these based on Target Type")]
    public TMP_InputField TargetInputField;
    public TextMeshProUGUI TargetText;

    public void TypeDigit(int digit)
    {
        string d = digit.ToString();

        switch (TargetType)
        {
            case NumpadTargetType.InputField:
                if (TargetInputField != null)
                    TargetInputField.text += d;
                break;

            case NumpadTargetType.TextMeshProUGUI:
                if (TargetText != null)
                    TargetText.text += d;
                break;
        }
    }

    public void DeleteLastDigit()
    {
        switch (TargetType)
        {
            case NumpadTargetType.InputField:
                if (TargetInputField != null && TargetInputField.text.Length > 0)
                    TargetInputField.text = TargetInputField.text[..^1];
                break;

            case NumpadTargetType.TextMeshProUGUI:
                if (TargetText != null && TargetText.text.Length > 0)
                    TargetText.text = TargetText.text[..^1];
                break;
        }
    }

    public void ClearAll()
    {
        switch (TargetType)
        {
            case NumpadTargetType.InputField:
                if (TargetInputField != null)
                    TargetInputField.text = string.Empty;
                break;

            case NumpadTargetType.TextMeshProUGUI:
                if (TargetText != null)
                    TargetText.text = string.Empty;
                break;
        }
    }
}
