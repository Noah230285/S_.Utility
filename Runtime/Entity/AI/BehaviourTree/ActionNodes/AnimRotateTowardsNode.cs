using _S.AI;
using _S.Utility;
using UnityEngine;

public class AnimRotateTowardsNode : ActionNode
{
    public float RotationSpeed;
    public float HorizontalOffset;
    public float HorizontalFOV;
    public float VerticalOffset;
    public float VerticalFOV;
    //public AnimationCurve RotationCurve;
    public string RotationAnimName;

    float previousH;
    float previousV;

    int lastFrames;

    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        Vector3 forward = agent.ChestRotationDummy.forward;
        Vector3 distance = agent._transformLookTarget.value.position - agent.ChestRotationDummy.position;

        float angleH = Vector3.SignedAngle(forward, distance.XZ().normalized, Vector3.up) + HorizontalOffset;
        //angleV = Vector3.SignedAngle(agent.ArmsRotationDummy.forward, (agent._transformLookTarget.value.position - agent.ArmsRotationDummy.position).normalized, agent.ArmsRotationDummy.right) + VerticalOffset;
        distance = (agent._transformLookTarget.value.position - agent.ArmsRotationDummy.position);
        float DotAngle = Vector3.Dot(distance.normalized.XZ(), distance.normalized);
        float angleV = Mathf.Acos(DotAngle) * Mathf.Sign(distance.y) * Mathf.Rad2Deg + VerticalOffset;

        if (Time.frameCount > lastFrames + 1)
        {
            previousH = angleH;
            previousV = angleV;
        }
        lastFrames = Time.frameCount;
        //Vector3 forward = agent._chestRotationDummy.forward.XZ().normalized;

        previousH = CalculateFinalAngle(angleH, previousH, HorizontalFOV);
        agent._animator.SetFloat($"{RotationAnimName}H", ConvertToPositiveSign(previousH) / 360);

        previousV = CalculateFinalAngle(angleV, previousV, VerticalFOV);
        agent._animator.SetFloat($"{RotationAnimName}V", ConvertToPositiveSign(previousV) / 360);

        return State.SUCCESS;
    }

    float CalculateFinalAngle(float currentAngle, float previousAngle, float FOVAngle)
    {
        previousAngle = Mathf.MoveTowards(previousAngle, currentAngle, Time.deltaTime * RotationSpeed);
        int sign = (int)Mathf.Sign(previousAngle);
        previousAngle = Mathf.Clamp(Mathf.Abs(previousAngle), 0, FOVAngle) * sign;
        return previousAngle;
    }

    float ConvertToPositiveSign(float previousAngle)
    {
        if (previousAngle > 360)
        {
            previousAngle = 360 - (previousAngle - 360);
        }
        else if (previousAngle < 0)
        {
            previousAngle = 360 + previousAngle;
            if (previousAngle < 0)
            {
                previousAngle = 360 + previousAngle;
            }
        }
        return previousAngle;
    }
}
