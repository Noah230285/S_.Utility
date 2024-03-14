using _S.AI;
using UnityEngine;

public class SetAnimMovementNode : ActionNode
{
    public float speedRange;
    public string MovementAnimName;
    Vector3 LastPosition;
    public override void OnStart()
    {
        LastPosition = agent.transform.position;
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        Vector3 Velocity = (agent.transform.position - LastPosition) / Time.deltaTime;
        float forward = Vector3.Project(Velocity, agent.transform.forward).magnitude;
        float right = Vector3.Project(Velocity, agent.transform.right).magnitude;
        agent._animator.SetFloat($"{MovementAnimName}Forward", forward);
        agent._animator.SetFloat($"{MovementAnimName}Right", right);
        LastPosition = agent.transform.position;
        return State.SUCCESS;
    }
}
