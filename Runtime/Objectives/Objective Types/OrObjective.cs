using UnityEngine;

namespace _S.Objectives
{
    public class OrObjective : ConditionalObjective
    {
        public override void StartObjective()
        {
            base.StartObjective();
            foreach (Objective condition in Conditions)
            {
                condition.StartObjective();
                condition.CallParentEnded = ConditionComplete;
            }
        }

        public override void ConditionComplete(Objective condition)
        {
            base.ConditionComplete(condition);
            if (condition.State == CompletionState.Inactive) { return; }
            if (condition.State == CompletionState.Succeeded)
            {
                OnEnded(CompletionState.Succeeded);
            }
            if (ConditionsMet == Conditions.Count)
            {
                OnEnded(CompletionState.Failed);
            }
        }
    }
}
