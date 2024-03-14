using UnityEngine;
using UnityEngine.AI;

namespace _S.AI.Actions
{
    public class ShootNode : ActionNode
    {
        public bool _fire = false;

        public override void OnStart()
        {
            
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            agent._fire = _fire;
            return State.SUCCESS;
        }
    }
}