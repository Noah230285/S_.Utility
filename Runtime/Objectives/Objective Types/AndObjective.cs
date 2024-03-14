namespace _S.Objectives
{
    public class AndObjective : ConditionalObjective
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
            if (condition.State == CompletionState.Failed)
            {
                OnEnded(CompletionState.Failed);
            }
            if (ConditionsMet == Conditions.Count)
            {
                OnEnded(CompletionState.Succeeded);
            }
        }
    }
}
