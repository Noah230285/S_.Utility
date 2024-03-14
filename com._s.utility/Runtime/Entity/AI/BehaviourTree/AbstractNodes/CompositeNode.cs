using System.Collections.Generic;

namespace _S.AI
{

    public abstract class CompositeNode : Node
    {
        public List<Node> Children = new();
        public override void OnStart()
        {
            Children.ForEach(n => n.OnStart());
        }

        public override void OnStop()
        {
            Children.ForEach(n => n.OnStop());
        }
    }
}
