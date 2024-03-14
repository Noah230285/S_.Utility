using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _S.Utility
{
    public class DestroyThisBehaviour : MonoBehaviour
    {
        public UnityAction destroyed;
        private void OnDestroy()
        {
            destroyed?.Invoke();
        }

        public void DestroyDelayed(float time)
        {
            StartCoroutine(DestroyDelayedEnum(time));
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }
        public void DestroyThis(GameObject gameObject)
        {
            Destroy(gameObject);
        }

        IEnumerator DestroyDelayedEnum(float time)
        {
            yield return new WaitForSeconds(time);
            DestroyThis(gameObject);
        }
    }
}