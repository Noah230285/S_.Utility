using UnityEngine;

namespace _S.AI.Actions
{
    public class WaitNode : ActionNode
    {
        [SerializeField] float _duration = 1f;
        float startTime;
        bool finished;

        public override void OnStart()
        {
            startTime = Time.time;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (!finished)
            {
                if (Time.time - startTime > _duration)
                {
                    startTime = Time.time;
                    finished = true;
                    return State.SUCCESS;
                }
            }
            else
            {
                finished = false;
            }

            return State.RUNNING;
        }
    }
}