using _S.AI;
using System;
using UnityEngine;

public class SetBoolNode : ActionNode
{
    public string BoolName;
    public bool SetValue;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        for (int i = 0; i < blackboard.bools.Length; i++)
        {
            if (blackboard.bools[i].Name == BoolName)
            {
                blackboard.bools[i].Enabled = SetValue;
                return State.SUCCESS;
            }
        }
        Debug.LogWarning($"Blackboard bool with name {BoolName} not found", this);
        return State.SUCCESS;
    }
}
