using _S.AI;
using UnityEngine;

public class CheckBoolNode : ConditionNode
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
                return blackboard.bools[i].Enabled ? State.SUCCESS : State.FAILURE;
            }
        }
        Debug.LogWarning($"Blackboard bool with name {BoolName} not found", this);
        return State.FAILURE;
    }
}
