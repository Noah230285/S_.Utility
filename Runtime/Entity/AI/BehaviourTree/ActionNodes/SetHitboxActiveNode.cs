using _S.AI;

public class SetHitboxActiveNode : ActionNode
{
    public bool Active;

    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override State OnUpdate()
    {
        agent.Hitbox.gameObject.SetActive(Active);
        return State.SUCCESS;
    }
}
