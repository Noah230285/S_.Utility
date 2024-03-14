using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _S.Objectives;
using System.Linq;

public class ObjectiveBrain : MonoBehaviour
{
    [SerializeField] ObjectiveManager _objectiveManager;
    [SerializeField] ObjectiveTree objectiveTree;
    [SerializeField] bool _AddOnStart = false;
    [SerializeField] bool _removeIfDestroyed = true;

    void Start()
    {
        if (_AddOnStart)
        {
            RegisterTreeToManager();
        }
    }

    void OnDestroy()
    {
        if (_removeIfDestroyed && _objectiveManager.objectives.Contains(objectiveTree))
        {
            RemoveTreeFromManager();
        }
    }

    public void RegisterTreeToManager()
    {
        if (_objectiveManager.objectives.Contains(objectiveTree))
        {
            Debug.LogWarning("Objective already registered", this);
            return;
        }
        objectiveTree.Remove += CompletedObjective;
        _objectiveManager.AddObjective(objectiveTree);
    }

    void RemoveTreeFromManager()
    {
        if (!_objectiveManager.objectives.Contains(objectiveTree))
        {
            Debug.LogWarning("Objective not registered", this);
            return;
        }
        objectiveTree.Remove -= CompletedObjective;
        _objectiveManager.RemoveObjective(objectiveTree);
    }

    public void CompleteEventObjective(EventObjective objective)
    {
        if (objectiveTree.Objectives.Contains(objective))
        {
            if (objective.State == Objective.CompletionState.InProgress)
            {
                objective.EventRaised();
            }
            else
            {
                Debug.LogWarning($"Passed objective is already completed", this);
            }
        }
        else
        {
            Debug.LogWarning($"Objective tree {objectiveTree.name} does not contain passed objective {objective}", this);
        }
    }

    void CompletedObjective(ObjectiveTree tree)
    {
        RemoveTreeFromManager();
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        //Gizmos.DrawGUITexture(new Rect( 100, 100, 100, 100), Texture."Assets/Textures/Editor Icons/brain.png");
    }
#endif
}
