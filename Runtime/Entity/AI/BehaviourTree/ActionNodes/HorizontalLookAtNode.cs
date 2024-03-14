using _S.AI;
using _S.Utility;
using Unity.VisualScripting;
using UnityEngine;

public class HorizontalLookAtNode : ActionNode
{
    public float RotationSpeed;
    public float RotationOffset;
    public AnimationCurve RotationCurve;

    float currentRotation;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        float angleH = Vector3.SignedAngle(Vector3.forward, (agent._transformReference.value.position.XZ() - agent.transform.position.XZ()).normalized, Vector3.up) + RotationOffset;

        currentRotation = Mathf.MoveTowardsAngle(agent.transform.eulerAngles.y, angleH, blackboard.deltaTime * RotationSpeed);
        agent.transform.eulerAngles = Vector3.up * currentRotation;
        return State.SUCCESS;
    }
}
