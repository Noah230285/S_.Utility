using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class ChaseNode : ActionNode
    {
        public Transform _transformTarget;
        public NavMeshAgent _agent;

        public override void OnStart()
        {
            _transformTarget = agent._transformReference.value;
            _agent = agent._NavMeshAgent;
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            _agent.SetDestination(_transformTarget.position);

            return State.SUCCESS;
        }
    }
}