using _S.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotNode : DecoratorNode
{
    public override State OnUpdate()
    {
        switch (Child.Update())
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.SUCCESS:
                return State.FAILURE;
            case State.FAILURE:
                return State.SUCCESS;
            default:
                return State.FAILURE;
        }
    }
}
