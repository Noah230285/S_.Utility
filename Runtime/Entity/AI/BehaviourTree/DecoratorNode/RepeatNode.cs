using _S.AI;

public class RepeatNode : DecoratorNode
{
    public override State OnUpdate()
    {
        Child.Update();
        return State.RUNNING;
    }
}
