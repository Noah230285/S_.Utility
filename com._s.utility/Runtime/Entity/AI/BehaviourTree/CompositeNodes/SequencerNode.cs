namespace _S.AI.CompositeNodes
{
    public class SequencerNode : CompositeNode
    {
        int current;
        public override State OnUpdate()
        {
            var child = Children[current];
            switch (child.Update())
            {
                case State.RUNNING:
                    current = 0;
                    return State.FAILURE;
                case State.SUCCESS:
                    current++;
                    break;
                case State.FAILURE:
                    current = 0;
                    return State.FAILURE;
            }
            if (current == Children.Count)
            {
                current = 0;
                return State.SUCCESS;
            }
            else
            {
                return OnUpdate();
            }
        }

        public override Node Clone()
        {
            SequencerNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}