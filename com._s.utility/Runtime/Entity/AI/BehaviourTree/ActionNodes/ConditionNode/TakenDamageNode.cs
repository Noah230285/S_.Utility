using UnityEngine;
using UnityEngine.AI;


namespace _S.AI.Actions
{
    public class TakenDamageNode : ConditionNode
    {
        public float HealthThreshold;
        float _oldHealth;

        public override void OnStart()
        {
            _oldHealth = agent._healthInterface.currentHealth;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            var health = agent._healthInterface.currentHealth;
            if (health < _oldHealth - HealthThreshold)
            {
                _oldHealth = health;
                return State.SUCCESS;
            }
            else
            {
                if (health > _oldHealth)
                {
                    _oldHealth = health;
                }
                return State.FAILURE;
            }
        }
    }
}