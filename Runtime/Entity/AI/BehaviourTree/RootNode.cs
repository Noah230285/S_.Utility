using _S.AI;
using UnityEngine;

public class RootNode : Node
{
    [HideInInspector] public Node Child;

    public override void OnStart()
    {
        Child.OnStart();
    }

    public override void OnStop()
    {
        Child.OnStop();
    }

    public override State OnUpdate()
    {
        return Child.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.Child = Child.Clone();
        return node;
    }
}
