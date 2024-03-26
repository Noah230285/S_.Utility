using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocumentExtender))]
public class UXMLRemmappingBinder : MonoBehaviour
{
    [Serializable]
    public class InputActionBinder
    {
        [SerializeField] string _inputName;
        [SerializeField] InputActionReference _actionReference;
        public InputActionReference actionReference => _actionReference;

        [SerializeField] InputBind _primary;
        public InputBind primary => _primary;
        public const string k_primaryButtonName = "KeyboardPrimary";

        [SerializeField] InputBind _secondary;
        public InputBind secondary => _secondary;
        public const string k_secondaryButtonName = "KeyboardSecondary";

        [SerializeField] InputBind _controller;
        public InputBind controller => _controller;
        public const string k_controllerButtonName = "Controller";

#if UNITY_EDITOR
        [SerializeField] bool _sectionExtended;
#endif

        public void BindUXML(UIDocument document)
        {
            VisualElement InputRemapper = document.rootVisualElement.Q(_inputName);
            primary.WriteInputOnUXML(InputRemapper, k_primaryButtonName, _inputName);
            secondary.WriteInputOnUXML(InputRemapper, k_secondaryButtonName, _inputName);
            controller.WriteInputOnUXML(InputRemapper, k_controllerButtonName, _inputName);
        }

        public void UnbindEvents()
        {
            primary.UnbindEvents();
            secondary.UnbindEvents();
            controller.UnbindEvents();
        }

        public void ToggleKeyboardBinding(bool enabled)
        {
            if (enabled)
            {
                primary.Enable();
                secondary.Enable();
                controller.Disable();
            }
            else
            {
                primary.Disable();
                secondary.Disable();
                controller.Enable();
            }
        }
    }
    [Serializable]
    public class InputBind
    {
        UXMLRemmappingBinder _rootRemapper;
        Button _button;
        public UXMLRemmappingBinder rootRemapper { set { _rootRemapper = value;} }

        [SerializeField] InputActionReference _actionReference;
        public InputActionReference actionReference => _actionReference;

        [SerializeField] int _bindingIndex;
        public int bindingIndex => _bindingIndex;

        [SerializeField] string _bindingID;
        public string bindingID => _bindingID;

        bool _hovered;
        enum UsabilityState
        {
            enabled,
            disabled,
            bricked,
        }
        UsabilityState _usabilityState;

        const string k_emptyText = "EMPTY";
        string _inputName;
        string _bindType;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        public void WriteInputOnUXML(VisualElement inputRemapper, string buttonName, string inputName)
        {
            if (_actionReference == null)
            {
                return;
            }

            _button = inputRemapper.Q(buttonName).ElementAt(0) as Button;
            if (bindingIndex == -1)
            {
                _button.focusable = false;
                _button.AddToClassList("disabled");
                _usabilityState = UsabilityState.bricked;
                return;
            }

            _button.RegisterCallback<MouseEnterEvent>(MouseEnter);
            void MouseEnter(MouseEnterEvent evt)
            {
                EnterHover();
            }
            _button.RegisterCallback<MouseLeaveEvent>(MouseLeave);
            void MouseLeave(MouseLeaveEvent evt)
            {
                LeaveHover();
            }
            _button.RegisterCallback<FocusInEvent>(Selected);
            void Selected(FocusInEvent evt)
            {
                EnterHover();
            }
            _button.RegisterCallback<FocusOutEvent>(UnSelected);
            void UnSelected(FocusOutEvent evt)
            {
                LeaveHover();
            }
            _button.clicked += MouseClick;
            void MouseClick()
            {
                StartInteractiveRebind();
            }

            _inputName = inputName;
            switch (buttonName)
            {
                case InputActionBinder.k_primaryButtonName:
                    _bindType = "primary";
                    break;
                case InputActionBinder.k_secondaryButtonName:
                    _bindType = "secondary";
                    break;
                case InputActionBinder.k_controllerButtonName:
                    _bindType = "controller";
                    break;
                default: break;
            }

            if (!(_actionReference.action.bindings.Count - 1 >= bindingIndex))
            {
                Debug.LogError($"Binding index overflow {_actionReference} {buttonName}", _rootRemapper);
            }

            UpdateButton();
        }

        public void UnbindEvents()
        {
            if (_hovered)
            {
                _hovered = false;
                _rootRemapper.emptyBindingActionReference.action.performed -= EmptyBinding;
                _rootRemapper.resetBindingActionReference.action.performed -= ResetBinding;
            }
        }

        void EnterHover()
        {
            if (_hovered) return;
            _rootRemapper.ReplaceHoverTarget(this);
            _hovered = true;
            if (_usabilityState == UsabilityState.enabled)
            {
                _rootRemapper.emptyBindingActionReference.action.performed += EmptyBinding;
                _rootRemapper.resetBindingActionReference.action.performed += ResetBinding;
            }
        }
        public void LeaveHover()
        {
            if (!_hovered) return;
            _hovered = false;
            if (_usabilityState == UsabilityState.enabled)
            {
                _rootRemapper.emptyBindingActionReference.action.performed -= EmptyBinding;
                _rootRemapper.resetBindingActionReference.action.performed -= ResetBinding;
            }
        }

        public void Enable()
        {
            if (_usabilityState == UsabilityState.bricked)
            {
                return;
            }
            _usabilityState = UsabilityState.enabled;
            _button.RemoveFromClassList("greyedOut");
        }

        public void Disable()
        {
            if (_usabilityState == UsabilityState.bricked)
            {
                return;
            }
            if (_hovered)
            {
                _rootRemapper.emptyBindingActionReference.action.performed -= EmptyBinding;
                _rootRemapper.resetBindingActionReference.action.performed -= ResetBinding;
            }
            _usabilityState = UsabilityState.disabled;
            _button.AddToClassList("greyedOut");
        }

        /// <summary>
        /// Empties this binding
        /// </summary>
        void EmptyBinding(InputAction.CallbackContext callback)
        {
            _rootRemapper.localRebind = true;
            _actionReference.action.ApplyBindingOverride(bindingIndex, "");
            UpdateButton();
        }

        /// <summary>
        /// Resets this binding to its initial state
        /// </summary>
        void ResetBinding(InputAction.CallbackContext callback)
        {
            InputAction action = _actionReference.action;
            InputBinding newBinding = action.bindings[bindingIndex];
            if (newBinding.overridePath == newBinding.path)
            {
                return;
            }

            _rootRemapper.localRebind = true;
            string oldOverridePath = newBinding.overridePath;
            if (oldOverridePath == null)
            {
                oldOverridePath = "";
            }
            action.RemoveBindingOverride(bindingIndex);

            if (newBinding.path == "")
            {
                UpdateButton();
                return;
            }

            if (_rootRemapper.SearchForDuplicates(this, out var otherBinding))
            {
                Debug.Log(otherBinding.bindingIndex);
                Debug.Log(oldOverridePath);

                otherBinding.actionReference.action.ApplyBindingOverride(otherBinding.bindingIndex, oldOverridePath);
                otherBinding.UpdateButton();
            }
            UpdateButton();
        }

        /// <summary>
        /// Updates the text or icon of this binder's button when the binding is changed
        /// </summary>
        public void UpdateButton()
        {
            //Debug.Log(_actionReference.action.bindings[bindingIndex].path);
            //Debug.Log(_actionReference.action.bindings[bindingIndex].effectivePath);

            if (bindingIndex == -1)
            {
                return;
            }

            var action = _actionReference?.action;
            if (action == null)
            {
                return;
            }
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // Get display string from action.
             displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, InputBinding.DisplayStringOptions.DontIncludeInteractions);

            if (displayString == "")
            {
                displayString = k_emptyText;
            }
            if (_button != null)
                _button.text = displayString;

            // Give listeners a chance to configure UI in response.
            _rootRemapper.OnUpdateBindingUXMLEvent(displayString, deviceLayoutName, controlPath);
        }

        /// <summary>
        /// Return the action and binding index for the binding that is targeted by the component
        /// according to
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = _actionReference?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(bindingID))
                return false;

            // Look up binding index.
            var bindingId = new Guid(bindingID);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (_usabilityState != UsabilityState.enabled)
            {
                return;
            }
            if (_rootRemapper.localRebind || _rootRemapper.doingRebinding)
            {
                return;
            }
            if (!ResolveActionAndBinding(out var action, out var bindingIndex)) return;
            PerformInteractiveRebind(action, bindingIndex);
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        void PerformInteractiveRebind(InputAction action, int bindingIndex)
        {
            _rebindOperation?.Cancel(); // Will null out _rebindOperation.

            action.Disable();

            // Action ran when the rebind is complete or canceled
            Action<InputActionRebindingExtensions.RebindingOperation> cleanUp = (operation) =>
            {
                _rootRemapper.OnRebindStopEvent(operation);
                _rootRemapper.DisableOverlay();

                _rebindOperation?.Dispose();
                _rebindOperation = null;
                _rootRemapper.doingRebinding = false;
                _rootRemapper.localRebind = true;

                UpdateButton();

                _rootRemapper.StartCoroutine(EnableRebindingDelay());
                IEnumerator EnableRebindingDelay()
                {
                    Debug.Log("delay");
                    yield return null;
                    action.Enable();
                    _rootRemapper.localRebind = false;
                }
            };

            InputAction exitAction = _rootRemapper.exitBindingPromptActionReference.action;
            InputBinding exitBind = new();
            string exitBindName = null;
            for (int i = 0; i < exitAction.bindings.Count; i++)
            {
                var bind = exitAction.bindings[i];
                if (bind.isPartOfComposite)
                {
                    continue;
                }
                if (_rootRemapper._UIDocumentExtender.controlScheme.Equals(bind.groups))
                {
                    exitBind = bind;
                    exitBindName = exitAction.GetBindingDisplayString(i);
                    break;
                }
            }

            // Configure the rebind.
            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(cleanUp)
                .OnComplete(operation =>
                {
                    cleanUp(operation);
                    if (_rootRemapper.SearchForDuplicates(this, out InputBind duplicateBind))
                    {
                        duplicateBind.EmptyBinding(new InputAction.CallbackContext());
                    }
                });
            if (exitBindName != null)
            {
                _rebindOperation.WithCancelingThrough(exitBind.effectivePath);
            }

            // Bring up rebind overlay, if we have one.
            _rootRemapper.EnableOverlay(_bindType, _inputName, exitBindName);
            _rootRemapper.localRebind = true;
            _rootRemapper.doingRebinding = true;

            _rootRemapper.OnRebindStartEvent(_rebindOperation);

            _rebindOperation.Start();
        }
    }

    public InputActionBinder[] _inputActionBinders;

    [SerializeField] InputActionReference _resetBindingActionReference;
    public InputActionReference resetBindingActionReference => _resetBindingActionReference;

    [SerializeField] InputActionReference _emptyBindingActionReference;
    public InputActionReference emptyBindingActionReference => _emptyBindingActionReference;

    [SerializeField] InputActionReference _exitBindingPromptActionReference;
    public InputActionReference exitBindingPromptActionReference => _exitBindingPromptActionReference;

    [Serializable]
    public class InteractiveUXMLRebindEvent : UnityEvent<UXMLRemmappingBinder, InputActionRebindingExtensions.RebindingOperation>
    {
    }

    [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
    + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
    + "customize the rebind.")]
    [SerializeField]
    private InteractiveUXMLRebindEvent _rebindStartEvent;
    public void OnRebindStartEvent(InputActionRebindingExtensions.RebindingOperation x)
    {
        _rebindStartEvent.Invoke(this, x);
    }

    [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
    [SerializeField]
    private InteractiveUXMLRebindEvent _rebindStopEvent;
    public void OnRebindStopEvent(InputActionRebindingExtensions.RebindingOperation x)
    {
        _rebindStartEvent.Invoke(this, x);
    }

    [Serializable]
    public class UpdateBindingUXMLEvent : UnityEvent<UXMLRemmappingBinder /*this*/, string /*displayString*/, string /*deviceLayoutName*/, string /*controlPath*/>
    {
    }

    [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
    + "bindings in custom ways, e.g. using images instead of text.")]
    [SerializeField]
    private UpdateBindingUXMLEvent _updateBindingUXMLEvent;

    /// <summary>
    /// Event that is triggered every time the UI updates to reflect the current binding.
    /// This can be used to tie custom visualizations to bindings.
    /// </summary>
    public UpdateBindingUXMLEvent updateBindingUXMLEvent
    {
        get
        {
            if (_updateBindingUXMLEvent == null)
                _updateBindingUXMLEvent = new UpdateBindingUXMLEvent();
            return _updateBindingUXMLEvent;
        }
    }

    public void OnUpdateBindingUXMLEvent(string displayString, string deviceLayoutName, string controlPath)
    {
        updateBindingUXMLEvent.Invoke(this, displayString, deviceLayoutName, controlPath);
    }

    bool _localRebind;
    public bool localRebind 
    {
        get => _localRebind;
        set { _localRebind = value; } 
    }

    bool _doingRebinding;
    bool doingRebinding
    {
        get => _doingRebinding;
        set { _doingRebinding = value; }
    }

    UIDocument _UIDocument;
    UIDocumentExtender _UIDocumentExtender;
    public VisualElement focusedElement => _UIDocumentExtender.currentlyFocused;

    VisualElement _bindingOverlay;
    Label _topLabel;
    Label _bottomLabel;

    InputBind _currentBinding;
    void Awake()
    {
        _UIDocument = GetComponent<UIDocument>();
        _UIDocumentExtender = GetComponent<UIDocumentExtender>();
    }
    void OnEnable()
    {
        if (!_UIDocumentExtender.isLoaded)
        {
            _UIDocumentExtender._UILoaded += Loaded;
            return;
        }
        Loaded();
    }

    void Loaded()
    {
        for (int i = 0; i < _inputActionBinders.Length; i++)
        {
            _inputActionBinders[i].primary.rootRemapper = this;
            _inputActionBinders[i].secondary.rootRemapper = this;
            _inputActionBinders[i].controller.rootRemapper = this;
            _inputActionBinders[i].BindUXML(_UIDocument);
        }

        _bindingOverlay = _UIDocument.rootVisualElement.Q(null, "bindingOverlay");
        if (_bindingOverlay == null)
        {
            Debug.LogError($"UI Document {_UIDocument} does not contain element with class 'bindingOverlay'", this);
        }
        _topLabel = _bindingOverlay.Q("TopLabel") as Label;
        _bottomLabel = _bindingOverlay.Q("BottomLabel") as Label;

        InputSystem.onActionChange += ActionChanged;
        _UIDocumentExtender.UsingGamepad += UsingGamepad;
        _UIDocumentExtender.UsingKeyboard += UsingKeyboard;
    }

    void OnDisable()
    {
        for (int i = 0; i < _inputActionBinders.Length; i++)
        {
            _inputActionBinders[i].UnbindEvents();
        }

        InputSystem.onActionChange -= ActionChanged;
        InputSystem.onActionChange += ActionChanged;
        _UIDocumentExtender.UsingGamepad -= UsingGamepad;
        _UIDocumentExtender.UsingKeyboard -= UsingKeyboard;
    }

    public void ActionChanged(object obj, InputActionChange change)
    {
        if (change != InputActionChange.BoundControlsChanged) return;
        if (_localRebind)
        {
            if (!doingRebinding)
            {
                _localRebind = false;
            }
            return;
        }
        var action = obj as InputAction;
        var actionMap = action?.actionMap ?? obj as InputActionMap;
        var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

        foreach (var element in _inputActionBinders)
        {
            var referencedAction = element.actionReference.action;
            if (element.actionReference == null)
            {
                continue;
            }

            if (referencedAction != action &&
                referencedAction.actionMap != actionMap &&
                referencedAction.actionMap?.asset != actionAsset)
            {
                continue;
            }

            element.primary.UpdateButton();
            element.secondary.UpdateButton();
            element.controller.UpdateButton();
        }
        if (actionAsset == null)
        {
            return;
        }
    }

    void UsingKeyboard()
    {
        foreach (var element in _inputActionBinders)
        {
            element.ToggleKeyboardBinding(true);
        }
    }
    void UsingGamepad()
    {
        foreach (var element in _inputActionBinders)
        {
            element.ToggleKeyboardBinding(false);
        }
    }

    public void ReplaceHoverTarget(InputBind inputBind)
    {
        _currentBinding?.LeaveHover();
        _currentBinding = inputBind;
    }

    public void EnableOverlay(string bindType, string bindName, string exitKey)
    {
        _bindingOverlay.RemoveFromClassList("hidden");
        _topLabel.text = $"Rebinding {bindType} input of '{bindName}'";

        if (exitKey != null)
        {
            _bottomLabel.text = $"Press any input to register or press {exitKey} to cancel";
        }
    }

    public void DisableOverlay()
    {
        _bindingOverlay.AddToClassList("hidden");
    }

    /// <summary>
    /// Searches for duplicate bindings paths of param<OriginRemap> and if it finds a duplicate, returns that InputBind param<searchForDuplicatesOut>
    /// </summary>
    /// <param name="OriginRemap"></param>
    /// <param name="searchForDuplicatesOut"></param>
    /// <returns>If duplicate was found</returns>
    bool SearchForDuplicates(InputBind OriginRemap, out InputBind searchForDuplicatesOut)
    {
        InputBinding OriginBinding = OriginRemap.actionReference.action.bindings[OriginRemap.bindingIndex];
        Func<InputActionBinder, InputBind, InputBind> compareBinding = (parentElement, compareInputBind) =>
        {
            if (compareInputBind == OriginRemap)
            {
                return null;
            }
            int bindingIndex = compareInputBind.bindingIndex;
            InputAction compareAction = parentElement.actionReference.action;
            if (bindingIndex == -1)
            {
                return null;
            }
            InputBinding compareBinding = compareAction.bindings[bindingIndex];

            if (compareBinding.effectivePath.Equals(OriginBinding.effectivePath))
            {
                return compareInputBind;
            }
            return null;
        };

        foreach (InputActionBinder element in _inputActionBinders)
        {
            searchForDuplicatesOut = compareBinding(element, element.primary);
            if (searchForDuplicatesOut != null) return true;

            searchForDuplicatesOut = compareBinding(element, element.secondary);
            if (searchForDuplicatesOut != null) return true;

            searchForDuplicatesOut = compareBinding(element, element.controller);
            if (searchForDuplicatesOut != null) return true;
        }
        searchForDuplicatesOut = null;
        return false;
    }
}
