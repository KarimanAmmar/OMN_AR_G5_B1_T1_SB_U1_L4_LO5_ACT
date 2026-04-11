using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDependency : MonoBehaviour,IDepndency
{
    [SerializeField] int count = 1;
    [ReadOnly] [ShowInInspector] private int index = 0;

    public bool IsCompleted()
    {
        return index >= count;
    }

    public void _TriggerCount() {
        index++;
    }
}
