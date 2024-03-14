using UnityEngine;
using UnityEngine.Events;

namespace _S.Hitboxes
{
    public class CallEventsHitbox : EmptyHitbox
    {
        public UnityEvent<Collider> EnterEvents, ExitEvents;
        protected override void Enter(Collider other)
        {
            if (_debug) { Debug.Log($"Enter {other}"); }
            EnterEvents.Invoke(other);
        }

        protected override void Exit(Collider other)
        {
            if (_debug) { Debug.Log($"Exit {other}"); }
            ExitEvents.Invoke(other);
        }
    }
}