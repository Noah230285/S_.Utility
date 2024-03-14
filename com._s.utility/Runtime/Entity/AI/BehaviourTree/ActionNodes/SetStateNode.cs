using _S.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStateNode : ActionNode
{
    public Blackboard.AIStates SetState;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        Debug.Log(SetState);
        blackboard.state = SetState;
        return State.SUCCESS;
    }
}
