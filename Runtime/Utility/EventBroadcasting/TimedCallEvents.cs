using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace _S.Utility.Broadcasting
{
    public class TimedCallEvents : MonoBehaviour
    {
        [SerializeField] UnityEvent _callEvents;

        [SerializeField] float _waitTime;

        void OnEnable()
        {
            StartCoroutine(WaitToCallEvents());
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnCallEvents()
        {
            _callEvents.Invoke();
        }

        IEnumerator WaitToCallEvents()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(_waitTime);
                OnCallEvents();
            }
        }
    }
}