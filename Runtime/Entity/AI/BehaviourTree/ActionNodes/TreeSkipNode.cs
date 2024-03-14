using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class TreeSkipNode : ActionNode
    {

        public bool setTreeSkip = false;

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            agent._treeSkip = setTreeSkip;
            return State.SUCCESS;
        }
    }
}