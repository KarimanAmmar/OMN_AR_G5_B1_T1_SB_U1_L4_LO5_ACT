using UnityEngine;
using UnityEngine.UI;

public enum NumpadActionType
{
    Digit0, Digit1, Digit2, Digit3, Digit4,
    Digit5, Digit6, Digit7, Digit8, Digit9,
    Delete
}

[RequireComponent(typeof(Button))]
public class NumpadAction : MonoBehaviour
{
    public Numpad TargetNumpad;
    public NumpadActionType ActionType;


    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Execute);
    }

    private void Execute()
    {
        if (ActionType == NumpadActionType.Delete)
            TargetNumpad.DeleteLastDigit();
        else
            TargetNumpad.TypeDigit((int)ActionType);
    }
}
