using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class IsDeadNode : ConditionNode
    {

        public override void OnStart()
        {
            
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            if (agent._healthInterface.currentHealth <= 0)
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