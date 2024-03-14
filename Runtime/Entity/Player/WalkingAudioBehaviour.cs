using _S.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace _S.Entity.Player
{
    [RequireComponent(typeof(AudioManager))]
    public class WalkingAudioBehaviour : MonoBehaviour
    {
        [SerializeField] Vector3 _direction = Vector3.down;
        [SerializeField] float _length;
        [SerializeField] LayerMask _layers;
        [SerializeField] bool _canHitTriggers;
        [SerializeField] PlayerController _controller;
        [SerializeField] AnimationCurve _movementSpeedVolumeCuve;

        AudioManager _audioManager;
        string _lastTag;
        RaycastHit _lastHit;

        public UnityEvent<RaycastHit> OnHit;
        public UnityEvent<RaycastHit> OnExit;

        private void Start()
        {
            _audioManager = GetComponent<AudioManager>();
        }
        void Update()
        {
            QueryTriggerInteraction triggerInteraction = _canHitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
            if (Physics.Raycast(transform.position - _direction.normalized * 0.01f, _direction, out RaycastHit hit, _length, _layers, triggerInteraction))
            {
                if (_lastTag != hit.collider.tag)
                {
                    _audioManager.Stop(_lastHit);
                    _audioManager.Play(hit);
                    _lastHit = hit;
                    _lastTag = hit.collider.tag;
                }
            }
            else
            {
                if (_lastTag != "")
                {
                    _audioManager.Stop(_lastHit);
                    _lastTag = "";
                }
            }
            float volumeMultiplier = 1;
            if (_controller != null)
            {
                volumeMultiplier = _movementSpeedVolumeCuve.Evaluate((_controller.horizontalVelocity).magnitude / _controller.config.MoveSpeed);
            }
            _audioManager.MultiplyBaseVolume(hit, volumeMultiplier);
        }
    }
}