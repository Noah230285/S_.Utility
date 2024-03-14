using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class IsFiringNode : ConditionNode
    {

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            if (agent._fire == true)
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