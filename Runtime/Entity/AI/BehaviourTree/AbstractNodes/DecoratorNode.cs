using UnityEngine;

namespace _S.AI
{
    public abstract class DecoratorNode : Node
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
        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}
