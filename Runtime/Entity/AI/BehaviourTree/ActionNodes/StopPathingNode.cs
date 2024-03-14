using UnityEngine;
using UnityEngine.AI;

namespace _S.AI.Actions
{
    public class StopPathingNode : ActionNode
    {
        public NavMeshAgent _agent;


        public override void OnStart()
        {
            _agent = agent._NavMeshAgent;
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            _agent.ResetPath();
            return State.SUCCESS;
        }
    }
}