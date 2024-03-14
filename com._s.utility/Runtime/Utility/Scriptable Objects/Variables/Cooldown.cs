using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace _S.Utility
{
    [Serializable]
    public class Cooldown
    {
        public bool active => progress > 0;
        public float Max;
        public float progress { get { return _progress; } set { SetProgess(value); } }
        public float progressPercent { get { return Max / _progress * 100.0f; } set { SetProgessPercent(value); } }

        [SerializeField] float _progress;
        [SerializeField] bool _paused = false;
        [SerializeField] bool _ticking = false;

        public event Action<CallbackContext> completed;


        public void Initialize()
        {
            _ticking = false;
            if (active)
            {
                StartTick();
            }
        }

        public void Start()
        {
            progress = Max;
            StartTick();
        }

        public void End()
        {
            if (progress > 0)
            {
                EndTick();
            }
            _progress = 0;
        }

        public void Pause()
        {
            EndTick();
            _paused = true;
        }

        public bool Resume()
        {
            _paused = false;
            return StartTick();
        }

        void SetProgess(float f = -1)
        {
            _progress = Mathf.Clamp(f, 0, Max);
            StartTick();
        }

        void SetProgessPercent(float f = -1)
        {
            SetProgess(Max * f / 100.0f);
        }

        public bool StartTick()
        {
            //Debug.Log("<color=red>try start</color>");
            if (_ticking || _paused || !Application.isPlaying) { return false; }
            //Debug.Log($"<color=red>start tick</color> {_ticking}");
            _ticking = true;
            CooldownTicker.Instance.CooldownTickEvents += Tick;
            return true;
        }

        void EndTick()
        {
            //Debug.Log($"<color=red>try end</color> {_ticking}");
            if (!_ticking || !Application.isPlaying) { return; }
            //Debug.Log("<color=red>end tick</color>");
            _ticking = false;
            CooldownTicker.Instance.CooldownTickEvents -= Tick;
        }

        void Tick(float time)
        {
            //Debug.Log($"tick {_ticking}");
            _progress = Mathf.MoveTowards(progress, 0, time);
            if (progress == 0)
            {
                EndTick();
                completed?.Invoke(new CallbackContext());
            }
        }
    }
}

