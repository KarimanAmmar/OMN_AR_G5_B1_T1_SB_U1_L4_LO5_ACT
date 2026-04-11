using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SimpleDependency : MonoBehaviour, IDepndency
{
    [SerializeField][ReadOnly] private bool isDepndencyDone = false;

    [SerializeField] public DelayType _delay = DelayType.None;
    [ShowIf(nameof(HasDelay))]
    [SerializeField] public float _delayInSeconds = 1;

    [Space(10)]

    [SerializeField] private UnityEvent2 OnComplete;

    public enum DelayType
    {
        Delay = 0 << 1, None = 1 << 1
    }

    private bool HasDelay()
    {
        return _delay != DelayType.None;
    }

    public bool IsCompleted()
    {
        return isDepndencyDone;
    }

    public void _Complete()
    {
        if (_delay != DelayType.None)
        {
            DOVirtual.DelayedCall(_delayInSeconds, () =>
            {
                isDepndencyDone = true;
                OnComplete?.Invoke();
            });
        }
        else
        {
            isDepndencyDone = true;
            OnComplete?.Invoke();
        }
    }

    public void _UnComplete()
    {
        isDepndencyDone = false;
    }
}

