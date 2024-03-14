using _S.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStateNode : ConditionNode
{
    public Blackboard.AIStates CheckState;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        if (blackboard.state == CheckState)
        {
            return State.SUCCESS;
        }
        return State.FAILURE;
    }
}
