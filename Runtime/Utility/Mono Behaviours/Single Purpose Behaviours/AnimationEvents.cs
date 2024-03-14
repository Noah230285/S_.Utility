using UnityEngine;
using UnityEngine.Events;

namespace _S.Utility
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEvents : MonoBehaviour
    {
        public UnityEvent[] m_unityEvents;

        Animator m_animator;

        private void Start()
        {
            m_animator = GetComponent<Animator>();
        }

        public void playEvents(int index)
        {
            if (m_unityEvents.Length <= index)
            {
                Debug.LogError($"AnimationEvents calling unity events at index {index} out of bounds of array", this);
                return;
            }
            //Debug.Log(gameObject.name, gameObject);
            m_unityEvents[index].Invoke();
        }
    }

}
