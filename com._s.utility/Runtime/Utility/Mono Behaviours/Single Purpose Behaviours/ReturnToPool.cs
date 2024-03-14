using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    public ObjectPool<GameObject> Pool;
    public float TimeToDisable = -1;

    void OnEnable()
    {
        if (TimeToDisable >= 0)
        {
            StartCoroutine(DisableTimer());
        }
    }
    void OnDisable()
    {
        Pool.Release(gameObject);
    }


    public void ReleaseThis()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator DisableTimer()
    {
        yield return new WaitForSeconds(TimeToDisable);
        ReleaseThis();
    }
}
