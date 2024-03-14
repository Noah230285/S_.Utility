using _S.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _S.Hitboxes
{
    public class ChangeHealthBehaviour : MonoBehaviour
    {
        [SerializeField] float _changeAmount;
        [SerializeField] float _waitTime;
        [SerializeField] float _changeTick;
        [SerializeField] bool _debug;

        List<HealthBehaviour> _healthBehaviours = new List<HealthBehaviour> { };
        List<Coroutine> _coroutines = new List<Coroutine> { };

        private void OnDisable()
        {
            _coroutines.Clear();
            _healthBehaviours.Clear();
            StopAllCoroutines();
        }
        public void AddHealthBehaviourOfCollider(Collider collider)
        {
            if (collider.transform.TryGetComponent(out DamageReciever dr))
            {
                HealthBehaviour hb = dr.healthReference;
                if (_healthBehaviours.Contains(hb)) { return; }
                if (_waitTime > 0)
                {
                    _coroutines.Add(StartCoroutine(WaitToDamage(hb)));
                }
                _healthBehaviours.Add(hb);
                hb.health += _changeAmount;
            }
            else if (_debug) Debug.LogWarning($"Could not find DamageReciever component to Add on {collider.gameObject.name}");
        }

        public void RemoveHealthBehaviourOfCollider(Collider collider)
        {
            if (collider.transform.TryGetComponent(out DamageReciever dr))
            {
                HealthBehaviour hb = dr.healthReference;
                if (_waitTime > 0)
                {
                    for (int i = 0; i < _healthBehaviours.Count; i++)
                    {
                        if (_healthBehaviours[i] == hb)
                        {
                            StopCoroutine(_coroutines[i]);
                            _coroutines.RemoveAt(i);
                        }
                    }
                }
                _healthBehaviours.Remove(hb);
            }
            else if (_debug) Debug.LogWarning($"Could not find DamageReciever component to Remove on {collider.gameObject.name}");
        }

        public void ChangeHealthBy(HealthBehaviour hb, float f)
        {
            hb.health += f;
        }


        IEnumerator WaitToDamage(HealthBehaviour hb)
        {
            while (enabled)
            {
                yield return new WaitForSeconds(_waitTime);
                ChangeHealthBy(hb, _changeTick);
            }
        }
    }
}