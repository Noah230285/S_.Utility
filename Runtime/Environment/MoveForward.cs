using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] float _speed;

    void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }
}
