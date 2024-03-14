using System.Diagnostics;
using UnityEngine;
namespace _S.AI.CompositeNodes
{
    public class SelectorNode : CompositeNode
    {
        public override State OnUpdate()
        {
            switch (Children[0].Update())
            {
                case State.RUNNING:
                    //UnityEngine.Debug.LogWarning("Sequencer not using condition node");
                    return State.FAILURE;
                case State.SUCCESS:
                    return Children[1].Update();
                case State.FAILURE:
                    return Children[2].Update();
                default:
                    return State.FAILURE;
            }
        }

        public override Node Clone()
        {
            SelectorNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}