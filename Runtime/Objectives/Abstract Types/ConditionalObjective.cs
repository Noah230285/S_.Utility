using _S.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _S.Objectives
{
    public abstract class ConditionalObjective : Objective
    {
        [HideInInspector] public List<Objective> Conditions = new List<Objective> { };
        [ReadOnly] public int ConditionsMet;
        public event Action<Objective> ConditionMet;

        public override void StartObjective()
        {
            base.StartObjective();
            int setDepth = ObjectiveText.Equals("") ? StartDepth : StartDepth + 1;
            foreach (var condition in Conditions)
            {
                condition.StartDepth = setDepth;
            }
        }

        public virtual void ConditionComplete(Objective completedCondition)
        {
            completedCondition.CallParentEnded = null;
            if (completedCondition.State == CompletionState.Inactive) { return; }
            ConditionsMet++;
            int add = completedCondition.ObjectiveText == "" ? 0 : 1;
            EndDepth = completedCondition.EndDepth + add;
            ConditionMet?.Invoke(this);
        }

        public override void OnEnded(CompletionState state)
        {
            foreach (Objective condition in Conditions)
            {
                if (condition.State == CompletionState.InProgress)
                {
                    condition.OnEnded(CompletionState.Inactive);
                }
            }
            base.OnEnded(state);
        }

        public override void ResetObjective()
        {
            base.ResetObjective();
            ConditionsMet = 0;
            foreach (Objective condition in Conditions)
            {
                condition.ResetObjective();
            }
        }

        public override int RegisterOrder(int previous)
        {
            ExecutionOrder = previous + 1;
            int truePrevious = previous + 1;
            foreach (var condition in Conditions)
            {
                truePrevious = condition.RegisterOrder(truePrevious);
            }
            return truePrevious;
        }

    }
}
