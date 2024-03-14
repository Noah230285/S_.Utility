using UnityEngine;
using UnityEngine.Events;

namespace _S.Utility.Broadcasting
{
    [CreateAssetMenu(menuName = "Utility/Event Channel")]
    public class ScriptableEventChannel : ScriptableObject
    {
        public UnityAction RaiseEvents;

        public void OnRaiseEvents()
        {
            RaiseEvents?.Invoke();
        }
    }
}
