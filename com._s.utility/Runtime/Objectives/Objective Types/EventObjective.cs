using _S.Attributes;
using _S.Utility.Broadcasting;
using System;
using UnityEngine;

namespace _S.Objectives
{
    public class EventObjective : Objective
    {
        public ScriptableEventChannel EventChannel;

        public int Needed = 1;
        [ReadOnly] public int Current;
        public event Action<Objective> Raised;
        string startText;
        public override void StartObjective()
        {
            startText = ObjectiveText;
            UpdateText();
            base.StartObjective();
            if (EventChannel != null)
            {
                EventChannel.RaiseEvents += EventRaised;
            }
        }

        public void EventRaised()
        {
            if (State == CompletionState.InProgress)
            {
                Current++;
                UpdateText();
                Raised?.Invoke(this);
                if (Current >= Needed) { OnEnded(CompletionState.Succeeded); }
            }
        }

        public override void ResetObjective()
        {
            base.ResetObjective();
            ObjectiveText = startText;
            State = CompletionState.Inactive;
            if (EventChannel != null)
            {
                EventChannel.RaiseEvents -= EventRaised;
            }
            Current = 0;
        }

        public void UpdateText()
        {
            int startBrackets = -1;
            bool inRaisedBrackets = false;
            for (int i = 0; i < ObjectiveText.Length; i++)
            {
                if (ObjectiveText[i] == '(')
                {
                    Debug.Log("a");
                    startBrackets = i;
                    continue;
                }
                if (startBrackets >= 0 && ObjectiveText[i] == '/')
                {
                    Debug.Log("b");
                    inRaisedBrackets = true;
                    continue;
                }
                if (inRaisedBrackets && ObjectiveText[i] == ')')
                {
                    Debug.Log("c");
                    if (i + 1 >= ObjectiveText.Length)
                    {
                        ObjectiveText = $"{ObjectiveText.Substring(0, startBrackets)}({Current}/{Needed})";
                    }
                    else
                    {
                        ObjectiveText = $"{ObjectiveText.Substring(0, startBrackets)}({Current}/{Needed}){ObjectiveText.Substring(i + 1, ObjectiveText.Length - 1)}";
                    }
                }
            }
        }
    }
}
