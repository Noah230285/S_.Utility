using _S.Attributes;
using System;
using System.Collections;

namespace _S.Objectives
{
    public class SequenceObjective : ConditionalObjective
    {
        [ReadOnly] public int CurrentCondition;
        public float Delay;
        public override void StartObjective()
        {
            UpdateText();
            base.StartObjective();
            StartNewCondition(0);
        }

        void StartNewCondition(int index)
        {
            Conditions[index].StartObjective();
            Conditions[index].CallParentEnded = ConditionComplete;
        }

        public override void ConditionComplete(Objective condition)
        {
            base.ConditionComplete(condition);
            CurrentCondition++;
            UpdateText();
            if (condition.State == CompletionState.Failed)
            {
                OnEnded(CompletionState.Failed);
            }
            if (CurrentCondition >= Conditions.Count)
            {
                OnEnded(CompletionState.Succeeded);
            }
            else
            {
                StartNewCondition(CurrentCondition);
            }
        }

        public override void ResetObjective()
        {
            base.ResetObjective();
            CurrentCondition = 0;
        }

        public void UpdateText()
        {
            int startBrackets = -1;
            bool inRaisedBrackets = false;
            for (int i = 0; i < ObjectiveText.Length; i++)
            {
                if (ObjectiveText[i] == '(')
                {
                    startBrackets = i;
                    continue;
                }
                if (startBrackets >= 0 && ObjectiveText[i] == '/')
                {
                    inRaisedBrackets = true;
                    continue;
                }
                if (inRaisedBrackets && ObjectiveText[i] == ')')
                {
                    if (i + 1 >= ObjectiveText.Length)
                    {
                        ObjectiveText = $"{ObjectiveText.Substring(0, startBrackets)}({CurrentCondition}/{Conditions.Count})";
                    }
                    else
                    {
                        ObjectiveText = $"{ObjectiveText.Substring(0, startBrackets)}({CurrentCondition}/{Conditions.Count}){ObjectiveText.Substring(i + 1, ObjectiveText.Length - 1)}";
                    }
                }
            }
        }
    }
}
