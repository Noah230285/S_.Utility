using _S.Entity;
using _S.Utility;
using UnityEngine;

namespace _S.Hitboxes
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class EmptyHitbox : MonoBehaviour
    {
        [SerializeField] bool _callExit;
        [SerializeField] bool _hitTriggers;
        [SerializeField] protected bool _debug;

        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger && !_hitTriggers) { return; }
            if (other.TryGetComponent(out DestroyThisBehaviour destroy))
            {
                if (_callExit)
                {
                    destroy.destroyed += () => Exit(other);
                }
            }
            Enter(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (!_callExit) { return; }
            if (other.isTrigger && !_hitTriggers) { return; }
            if (other.TryGetComponent(out DestroyThisBehaviour destroy))
            {
                destroy.destroyed -= () => Exit(other);
            }
            Exit(other);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.isTrigger && !_hitTriggers) { return; }
            if (collision.collider.TryGetComponent(out DestroyThisBehaviour destroy))
            {
                if (_callExit)
                {
                    destroy.destroyed += () => Exit(collision.collider);
                }
            }
            Enter(collision.collider);
        }

        void OnCollisionExit(Collision collision)
        {
            if (!_callExit) { return; }
            if (collision.collider.isTrigger && !_hitTriggers) { return; }
            if (collision.collider.TryGetComponent(out DestroyThisBehaviour destroy))
            {
                destroy.destroyed -= () => Exit(collision.collider);
            }
            Exit(collision.collider);
        }

        protected abstract void Enter(Collider other);
        protected abstract void Exit(Collider other);
    }
}