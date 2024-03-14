using UnityEngine;
using UnityEngine.AI;

namespace _S.AI.Actions
{
    public class DistanceToNode : ConditionNode
    {
        public Transform _transformTarget;
        public Transform _transform;
        public NavMeshAgent _agent;
        public float Distance = 5f;
        public bool Invert = false;

        public override void OnStart()
        {
            _transformTarget = agent._transformReference.value;
            _agent = agent._NavMeshAgent;
            _transform = agent._transform;
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            if (Invert == true)
            {
                if (Vector3.Distance(_transformTarget.position, _transform.position) >= Distance)
                {
                    _agent.ResetPath();
                    return State.FAILURE;
                }
                else
                {
                    return State.SUCCESS;
                }
            }
            else
            {
                if (Vector3.Distance(_transformTarget.position, _transform.position) <= Distance)
                {
                    _agent.ResetPath();
                    return State.FAILURE;
                }
                else
                {
                    return State.SUCCESS;
                }
            }
        }
    }
}