using _S.AI;
using UnityEngine.UIElements;
using UnityEngine;
using _S.Utility;

public class FOVNode : ConditionNode
{
    public float FOVAngle;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        float angle = Vector3.Angle(agent.transform.forward, agent._transformLookTarget.value.position.XZ() - agent.transform.position.XZ());
        if (angle <= FOVAngle)
        {
            return State.SUCCESS;
        }
        return State.FAILURE;
    }
}
