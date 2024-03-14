using _S.AI;
using UnityEngine;

public class AimAtTargetNode : ActionNode
{
    public float RotationSpeed;
    //public AnimationCurve RotationCurve;

    int lastFrames;
    Quaternion startLocalRotation;
    public override void OnStart()
    {
        startLocalRotation = agent.AimPoint.localRotation;
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        if (Time.frameCount > lastFrames + 1)
        {
            agent.AimPoint.LookAt(agent._transformLookTarget.value.position - Vector3.up * 0.5f);
            //agent.AimPoint.localRotation = startLocalRotation;
        }
        lastFrames = Time.frameCount;

        //Vector3 tar = new Vector3(0f, (float)agent._transformLookTarget.value.position.y, 0f);
        Quaternion lastRotation = agent.AimPoint.rotation;
        agent.AimPoint.LookAt(agent._transformLookTarget.value.position - Vector3.up * 0.5f);
        //agent.AimPoint.localEulerAngles = new Vector3(agent.AimPoint.localEulerAngles.x, 180, 0);
        agent.AimPoint.rotation = Quaternion.RotateTowards(lastRotation, agent.AimPoint.rotation, blackboard.deltaTime * RotationSpeed);
        //float angle = Vector3.SignedAngle(Vector3.forward, (agent._transformLookTarget.value.position - agent.AimPoint.position).normalized, agent.AimPoint.right);
        Debug.DrawRay(agent.AimPoint.transform.position, agent.AimPoint.transform.forward);

        //float ADotB = Vector3.Dot((agent.AimPoint.transform.position - agent.AimPoint.position).normalized, (new Vector3(agent._transformLookTarget.value.position.x, agent.AimPoint.position.y, agent._transformLookTarget.value.position.z) - agent.AimPoint.position).normalized);
        //float radians = Mathf.Acos(ADotB);
        //angle =  radians * Mathf.Rad2Deg;
        //currentRotation = Mathf.MoveTowardsAngle(agent.AimPoint.eulerAngles.x, angle, blackboard.deltaTime * RotationSpeed);
        //agent.AimPoint.eulerAngles = agent.AimPoint.eulerAngles.YZ() + Vector3.right * (currentRotation + RotationOffset);
        return State.SUCCESS;
    }
}
