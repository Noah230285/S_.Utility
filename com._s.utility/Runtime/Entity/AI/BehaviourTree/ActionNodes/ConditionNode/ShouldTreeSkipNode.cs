using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class ShouldTreeSkipNode : ConditionNode
    {

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            if (agent._treeSkip == true)
            {
                return State.SUCCESS;
            }
            else
            {
                return State.FAILURE;
            }
            
        }
    }
}