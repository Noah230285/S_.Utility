using UnityEngine;
using UnityEngine.Events;

namespace _S.Hitboxes
{
    public class HitboxReciever : MonoBehaviour
    {
        public UnityEvent<Collider> HitEnter, HitExit;

        public void OnHitEnter(Collider collider = null)
        {
            HitEnter.Invoke(collider);
        }

        public void OnHitExit(Collider collider = null)
        {
            HitExit.Invoke(collider);
        }
    }
}