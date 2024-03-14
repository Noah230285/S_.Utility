using _S.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace _S.Objectives
{
    //[CreateAssetMenu(menuName = "Utility/Objective")]
    public abstract class Objective : ScriptableObject
    {
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public int StartDepth;
        [HideInInspector] public int EndDepth;
        [HideInInspector] public Objective Parent;
        [HideInInspector] public int ExecutionOrder;

        [TextArea] public string ObjectiveText;
        public enum CompletionState
        {
            Inactive,
            InProgress,
            Failed,
            Succeeded
        }
        [ReadOnly] public CompletionState State;

        public event Action<Objective> Started;
        public event Action<Objective> Ended;
        public Action<Objective> CallParentEnded;

        public void ResetEnded()
        {
            Ended = null;
        }

        public virtual void OnEnded(CompletionState state)
        {
            State = state;
            Ended?.Invoke(this);
            CallParentEnded?.Invoke(this);
        }

        public virtual void StartObjective()
        {
            Started?.Invoke(this);
            State = CompletionState.InProgress;
        }
        public virtual void ResetObjective()
        {
            //if (State == CompletionState.InProgress)
            //{
            //    OnEnded(CompletionState.Inactive);
            //}
            State = CompletionState.Inactive;
            StartDepth = 0;
            EndDepth = 0;
            ExecutionOrder = 0;
            Started = null;
            Ended = null;
            CallParentEnded = null;
        }

        public virtual Objective Clone()
        {
            return Instantiate(this);
        }

        public virtual int RegisterOrder(int previous)
        {
            ExecutionOrder = previous + 1;
            return ExecutionOrder;
        }
    }
}
//#if UNITY_EDITOR
//        public void AddCondition()
//        {
//            System.Type type = typeof(ConditionalObjective);
//            ConditionalObjective condition = ScriptableObject.CreateInstance(type) as ConditionalObjective;
//            condition.name = type.Name;
//            condition.parent = this;
//            condition.Ended += CompletedCondition;
//            Undo.RecordObject(this, "Objective (AddCondition)");
//            _conditions.Add(condition);
//            if (!Application.isPlaying)
//            {
//                AssetDatabase.AddObjectToAsset(condition, this);
//            }
//            Undo.RegisterCreatedObjectUndo(condition, "Objective (AddCondition)");

//            AssetDatabase.SaveAssets();
//            Debug.Log(_conditions.Count);
//        }

//        public void RemoveCondition(ConditionalObjective condition)
//        {
//            Debug.Log("test");
//            Undo.RecordObject(this, "Objective (RemoveCondition)");
//            _conditions.Remove(condition);

//            Undo.DestroyObjectImmediate(condition);
//            AssetDatabase.SaveAssets();
//        }
//#endif
//    }
