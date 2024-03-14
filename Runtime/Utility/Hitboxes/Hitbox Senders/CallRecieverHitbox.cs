using System;
using UnityEngine;

namespace _S.Hitboxes
{
    [Serializable]
    public class CallRecieverHitbox : EmptyHitbox
    {
        Collider _collider;

        void Start()
        {
            _collider = GetComponent<Collider>();

        }

        protected override void Enter(Collider other)
        {
            if (other.TryGetComponent(out HitboxReciever reciever))
            {
                reciever.OnHitEnter(_collider);
            }
            else if (_debug)
            {
                Debug.LogWarning($"{this} cannot find a hitbox reciever in {other.gameObject}");
            }
        }

        protected override void Exit(Collider other)
        {
            if (other.TryGetComponent(out HitboxReciever reciever))
            {
                reciever.OnHitEnter(_collider);
            }
            else if (_debug)
            {
                Debug.LogWarning($"{this} cannot find a hitbox reciever in {other.gameObject}");
            }
        }
    }
}