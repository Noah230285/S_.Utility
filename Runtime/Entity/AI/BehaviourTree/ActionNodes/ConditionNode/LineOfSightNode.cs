using UnityEngine;
using UnityEngine.AI;

namespace _S.AI.Actions
{
    public class LineOfSightNode : ConditionNode
    {
        private Transform _transform;
        public NavMeshAgent _agent;
        public int _raycastDistance = 1000;
        public LayerMask _layermask;

        public override void OnStart()
        {
            _agent = agent._NavMeshAgent;
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            _transform = agent._transformLookTarget.value ?? agent._transformReference.value;
            RaycastHit hit;
            if (Physics.Raycast(agent.AimPoint.position, _transform.position - agent.AimPoint.position, out hit, Vector3.Magnitude(_transform.position - agent.AimPoint.position), _layermask))
            {
                return State.FAILURE;
            }
            else
            {
                return State.SUCCESS;
            }
        }
    }
}