using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableEvents : MonoBehaviour
{
    public UnityEvent Enabled;
    public float Delay;

    void OnEnable()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(Delay);
        Enabled.Invoke();
    }
}
