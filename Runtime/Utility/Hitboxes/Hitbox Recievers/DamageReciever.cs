using _S.Entity;
using UnityEngine;
namespace _S.Hitboxes
{
    public class DamageReciever : MonoBehaviour
    {
        [SerializeField] HealthBehaviour _healthReference;
        public HealthBehaviour healthReference => _healthReference;

        public void ChangeHealthBy(float change)
        {
            _healthReference.health += change;
        }
    }
}