using _S.Utility;
using UnityEngine;

namespace _S.Utility.Broadcasting
{
    public class BroadcastEvent : MonoBehaviour
    {
        [SerializeField] ScriptableEventChannel channel;

        public void Broadcast()
        {
            channel?.RaiseEvents();
        }
    }
}