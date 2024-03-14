using UnityEngine;

namespace _S.AI.Actions
{
    public class WaitRandomNode : ActionNode
    {
        [SerializeField] float _minDuration = 0f;
        [SerializeField] float _maxDuration = 1f;

        float _currentDuration;
        bool _finished;

        public override void OnStart()
        {
            _currentDuration = Random.Range(_minDuration, _maxDuration);
        }
        public override State OnUpdate()
        {
            if (_currentDuration < 0)
            {
                _currentDuration = Random.Range(_minDuration, _maxDuration);
                return State.SUCCESS;
            }
            else
            {
                _currentDuration -= Time.deltaTime;
            }
            return State.RUNNING;
        }
    }
}