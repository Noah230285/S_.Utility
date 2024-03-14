using System;
using UnityEngine;

namespace _S.Objectives
{
    public class RootObjective : Objective
    {
        [HideInInspector] public Objective Child;
        //public override void OnStarted()
        //{
        //    Child.OnStarted();
        //    base.OnStarted();
        //}
        public override Objective Clone()
        {
            RootObjective objective = Instantiate(this);
            objective.Child = Child.Clone();
            return objective;
        }

        public override void StartObjective()
        {
            base.StartObjective();
            ExecutionOrder = 0;
            Child.RegisterOrder(ExecutionOrder);
            int setDepth = ObjectiveText.Equals("") ? StartDepth : StartDepth + 1;
            Child.StartDepth = setDepth;
            Child.CallParentEnded = ChildCompleted;
            Child.StartObjective();
        }

        public void ChildCompleted(Objective objective)
        {
            int add = objective.ObjectiveText == "" ? 0 : 1;
            EndDepth = objective.EndDepth + add;
            OnEnded(objective.State);
        }

        public override void OnEnded(CompletionState state)
        {
            Child.CallParentEnded = ChildCompleted;
            base.OnEnded(state);
        }

        public override void ResetObjective()
        {
            base.ResetObjective();
            Child.ResetObjective();
        }
    }
}
