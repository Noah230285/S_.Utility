using _S.Attributes;
using _S.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _S.Interacting
{
    public class PlayerInteract : MonoBehaviour
    {

        [SerializeField] InputActionReference _interactButton;
        [SerializeField] float _interactDistance;

        [SerializeField, Section("interactableLayers", new string[] { "_interactableLayer", "_canInteractOutlineLayer", "_notInteractOutlineLayer" })] bool _interactableLayersExtended;
        [SerializeField, HideInInspector] int _interactableLayer;
        [SerializeField, HideInInspector] int _canInteractOutlineLayer;
        [SerializeField, HideInInspector] int _notInteractOutlineLayer;
        [SerializeField] Camera _playerCamera;
        Interactable _lastHover;

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(_playerCamera.transform.position, _playerCamera.transform.forward * _interactDistance, Color.red);
            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, 1 << _interactableLayer | 1 << _canInteractOutlineLayer | 1 << _notInteractOutlineLayer, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponent<Interactable>() == null)
                {
                    Debug.LogError("Object with interactable layer must have interactable script", hit.collider.gameObject);
                    return;
                }
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable.outlineMesh == null)
                    Debug.LogWarning("Interactable objects should have an outline mesh", interactable);
                else
                    interactable.outlineMesh.layer = interactable.canInteract ? _canInteractOutlineLayer : _notInteractOutlineLayer;
                if (_lastHover != interactable)
                {
                    UninteractLast();
                    interactable.HoverEnter();
                    _lastHover = interactable;
                }
                if (_interactButton.action.WasReleasedThisFrame())
                {
                    interactable.OnInteract(this);
                }
            }
            else
            {
                UninteractLast();
            }
        }

        void UninteractLast()
        {
            if (_lastHover != null)
            {
                if (_lastHover.outlineMesh == null)
                    Debug.LogWarning("Interactable objects should have an outline mesh", _lastHover);
                else
                    _lastHover.outlineMesh.layer = _interactableLayer;
                _lastHover.HoverExit();
                _lastHover = null;
            }
        }
    }
}
