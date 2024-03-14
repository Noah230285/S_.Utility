using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class PlayAnimationNode : ActionNode
    {
        public string _animationName;

        public override void OnStart()
        {
            
        }

        public override void OnStop()
        {
            
        }

        public override State OnUpdate()
        {
            agent._animator.SetTrigger(_animationName);
            return State.SUCCESS;
        }
    }
}