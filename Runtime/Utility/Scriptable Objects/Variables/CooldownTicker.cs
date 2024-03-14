using System;
using UnityEngine;

namespace _S.Utility
{
    public class CooldownTicker : MonoBehaviour
    {
        public static CooldownTicker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new("Cooldown Manager (automatically added)");
                    return _instance = obj.AddComponent<CooldownTicker>();
                }
                else
                {
                    return _instance;
                }
            }
            private set { _instance = value; }
        }
        static CooldownTicker _instance;

        public Action<float> CooldownTickEvents;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        // Update is called once per frame
        void Update()
        {
            CooldownTickEvents?.Invoke(Time.deltaTime);
        }
    }
}