using _S.Objectives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveReceiver : MonoBehaviour
{
    [Serializable]
    public struct ObjectiveRecieverInfo
    {
        public Objective Objective;
        public float delay;
        public UnityEvent Completed;
    }
    public ObjectiveRecieverInfo[] CatalogedObjectives;
    void OnEnable()
    {
        foreach (var info in CatalogedObjectives)
        {
            info.Objective.Ended += OnCompleted;
        }
    }
    void OnDisable()
    {
        foreach (var info in CatalogedObjectives)
        {
            info.Objective.Ended -= OnCompleted;
        }
    }

    public void OnCompleted(Objective objective)
    {
        var info = CatalogedObjectives.First(i => i.Objective == objective);
        StartCoroutine(Wait(info));
    }
    IEnumerator Wait(ObjectiveRecieverInfo info)
    {
        yield return new WaitForSeconds(info.delay);
        info.Completed.Invoke();
    }
}
