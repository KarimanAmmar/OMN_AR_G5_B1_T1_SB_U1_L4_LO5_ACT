using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventEntry
{
    public string name;

    public UnityEvent2 onEvent;

    public bool fireOnce;
    public bool useDelay;
    public float delay;

    [HideInInspector] public bool fired;
}


public class EventsHelper : MonoBehaviour
{
    [Header("Event List")]
    [SerializeField] private List<EventEntry> events = new List<EventEntry>();

    private int currentIndex = 0;

    /// <summary>
    /// Invoke current event if validation allows it
    /// </summary>
    public void InvokeCurrentEvent()
    {
        if (currentIndex < 0 || currentIndex >= events.Count)
            return;

        EventEntry entry = events[currentIndex];

        if (entry.fireOnce && entry.fired)
            return;

        if (entry.useDelay)
            StartCoroutine(InvokeWithDelay(entry));
        else
            InvokeEvent(entry);
    }

    /// <summary>
    /// Call this when your "next event validation" succeeds
    /// </summary>
    public void ValidateAndGoNext()
    {
        currentIndex++;

        if (currentIndex >= events.Count)
            currentIndex = events.Count - 1; // clamp
    }

    /// <summary>
    /// Optional: reset everything
    /// </summary>
    public void ResetAll()
    {
        currentIndex = 0;
        foreach (var e in events)
            e.fired = false;
    }

    private void InvokeEvent(EventEntry entry)
    {
        entry.onEvent?.Invoke();
        entry.fired = true;
    }

    private IEnumerator InvokeWithDelay(EventEntry entry)
    {
        yield return new WaitForSeconds(entry.delay);

        if (entry.fireOnce && entry.fired)
            yield break;

        InvokeEvent(entry);
    }
    public void ValidateAndInvokeCurrent()
    {
        ValidateAndGoNext();
        InvokeCurrentEvent();
    }
}