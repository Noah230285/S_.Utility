using UnityEngine;

namespace _S.AI
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            RUNNING,
            SUCCESS,
            FAILURE,
        }
        [HideInInspector] public State state;
        [HideInInspector] public  bool started = false;
        [HideInInspector] public bool active;

        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;

        [HideInInspector] public Blackboard blackboard;
        [HideInInspector] public AIAgent agent;

        [TextArea] public string description;

        public virtual State Evaluate() => State.FAILURE;

        public virtual State Update()
        {
            active = true;
            //if (!started)
            //{
            //    OnStart();
            //    started = true;
            //}
            state = OnUpdate();

            //if (state == State.FAILURE || state == State.SUCCESS)
            //{
            //    OnStop();
            //    started = false;
            //}
            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }

        public abstract State OnUpdate();
    }
}
