using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ActionSequenceController : MonoBehaviour
{
    [Header("Action Sequences")]
    public List<ActionSequence> actionSequences;


    public void StartActionSequence(string actionId)
    {
        StartCoroutine(PlayActionSequence(actionId));
    }


    private IEnumerator PlayActionSequence(string actionId)
    {
        var actionSequence = actionSequences.Find(a => a.SequenceId == actionId);

        if (actionSequence != null && actionSequence.activities != null)
        {
            foreach (var activity in actionSequence.activities)
            {
                yield return new WaitForSeconds(activity.delayBefore);

                activity.action?.Invoke();

                yield return new WaitForSeconds(activity.delayAfter);

                activity.onActivityFinished?.Invoke();
            }
        }
    }







    [System.Serializable]
    public class ActionSequence
    {
        public string SequenceId;
        public List<Activity> activities;
    }


    [System.Serializable]
    public class Activity
    {
        public float delayBefore;
        public UnityEvent2 action;
        public float delayAfter;
        public UnityEvent2 onActivityFinished;
    }
}