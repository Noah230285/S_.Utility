using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Blackboard
{
    public enum AIStates
    {
        Idle,
        Walking,
        Staggered,
        Stunned,
        Attacking
    }
    [SerializeField] AIStates _state;
    public AIStates state
    {
        get { return _state; }
        set { _state = value; }
    }
    public float deltaTime;
    public Vector3 moveToPosition;
    public GameObject moveToObject;

    public BlackboardBool[] bools = new BlackboardBool[0];
}
