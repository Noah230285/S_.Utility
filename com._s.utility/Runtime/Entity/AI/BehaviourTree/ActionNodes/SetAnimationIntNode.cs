using _S.AI;

public class SetAnimationIntNode : ActionNode
{
    public string AnimationName;
    public int SetInt;
    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        agent._animator.SetInteger(AnimationName, SetInt);
        return State.SUCCESS;
    }
}
