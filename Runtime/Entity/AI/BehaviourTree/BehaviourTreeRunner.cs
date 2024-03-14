using _S.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // Start is called before the first frame update
    void Awake()
    {
        tree = tree.Clone();
        tree.Bind(GetComponent<AIAgent>());
    }

    void OnEnable()
    {
        tree.Start();
    }

    void OnDisable()
    {
        tree.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        tree.blackboard.deltaTime = Time.deltaTime;
        tree.Update();
    }

    public void SetBlackboardBoolTrue(string name)
    {
        for (int i = 0; i < tree.blackboard.bools.Length; i++)
        {
            if (tree.blackboard.bools[i].Name == name)
            {
                tree.blackboard.bools[i].Enabled = true;
                return;
            }
        }
        Debug.LogWarning($"Blackboard bool with name {name} not found", this);
    }

    public void SetBlackboardBoolFalse(string name)
    {
        for (int i = 0; i < tree.blackboard.bools.Length; i++)
        {
            if (tree.blackboard.bools[i].Name == name)
            {
                tree.blackboard.bools[i].Enabled = false;
                return;
            }
        }
        Debug.LogWarning($"Blackboard bool with name {name} not found", this);
    }
}
