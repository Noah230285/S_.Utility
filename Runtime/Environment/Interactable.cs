using _S.ScriptableVariables;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _S.Interacting
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] GameObject _outlineMesh;
        public GameObject outlineMesh => _outlineMesh;

        [SerializeField] bool _disableAfterPress = true;
        [SerializeField] float _pressCooldown;

        public UnityEvent PressingButton;
        public UnityEvent HoveringButton;
        public UnityEvent EndHoveringButton;

        [SerializeField] bool _canInteract = true;
        [SerializeField] BoolVariable[] _interactRequirements;

        public bool canInteract
        {
            get 
            {
                for (int i = 0; i < _interactRequirements.Length; i++)
                {
                    if (!_interactRequirements[i].Value) return false;
                }
                return _canInteract;
            }
            set { _canInteract = value; }
        }
        int _animPress = 0;
        int _animHold = 0;
        Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator)
            {
                for (int i = 0; i < _animator.parameters.Length; i++)
                {
                    if (_animator.parameters[i].type == AnimatorControllerParameterType.Trigger)
                        _animPress = _animator.parameters[i].nameHash;
                    if (_animator.parameters[i].type == AnimatorControllerParameterType.Bool)
                        _animHold = _animator.parameters[i].nameHash;
                }
            }
        }

        public bool OnInteract(PlayerInteract player)
        {
            if (!canInteract) return false;

            PressingButton.Invoke();
            if (_disableAfterPress)
            {
                _canInteract = false;
            }
            else if (_pressCooldown > 0)
            {
                _canInteract = false;
                StartCoroutine(WaitForRenable());
            }
            if (_animPress != 0)
                _animator.SetTrigger(_animPress);
            return true;
        }

        public void AllowInteract()
        {
            _canInteract = true;
        }

        public void HoverEnter()
        {
            if (_animHold != 0)
                _animator.SetBool(_animHold, true);
            HoveringButton.Invoke();
        }

        public void HoverExit()
        {
            if (_animHold != 0)
                _animator.SetBool(_animHold, false);
            EndHoveringButton.Invoke();
        }

        IEnumerator WaitForRenable()
        {
            yield return new WaitForSeconds(_pressCooldown);
            canInteract = true;
        }
    }
}