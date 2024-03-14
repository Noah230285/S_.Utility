using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using _S.Utility;
using _S.Editor.UIToolkitExtras;

namespace _S.Editor.UXMLElements
{
    public class CooldownElement : VisualElement
    {
        #region
        public new class UxmlFactory : UxmlFactory<CooldownElement> { }
        public CooldownElement()
        {
            Init();
        }
        #endregion

        VisualElement _body;

        Button _pauseButton;
        VisualElement _pauseButtonIcon;

        Label _label;
        ProgressBar _bar;
        Slider _slider;
        FloatField _progressField;
        FloatField _maxField;

        SerializedProperty _cooldownReference;
        SerializedProperty _maxReference;
        SerializedProperty _progressReference;
        SerializedProperty _paused;

        Cooldown _cooldown;

        public Texture2D _pause => AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Textures/Editor Icons/Pause.png");
        public Texture2D _unpause => AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Textures/Editor Icons/Unpause.png");

        public CooldownElement(string label, SerializedProperty cooldownProperty)
        {
            Init(label, cooldownProperty);
        }

        public void Init(string labelText = "Default Label", SerializedProperty cooldownProperty = null)
        {
            // Load the source UXML file
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Editor/UIToolkit/UXML/Templates/cooldown.uxml");
            asset.CloneTree(this);

            // Find all the relevant Visual Elements
            _body = this.Q<VisualElement>("cooldown");
            _pauseButton = _body.ElementAt(0).ElementAt(0) as Button;
            _pauseButtonIcon = _body.ElementAt(0).ElementAt(0).ElementAt(0);

            _label = _body.ElementAt(1).ElementAt(0).ElementAt(0) as Label;

            _bar = _body.ElementAt(1).ElementAt(0).ElementAt(1) as ProgressBar;
            _slider = _body.ElementAt(1).ElementAt(0).ElementAt(1).ElementAt(1) as Slider;
            _progressField = _body.ElementAt(1).ElementAt(0).ElementAt(2) as FloatField;

            _maxField = _body.ElementAt(1).ElementAt(1) as FloatField;

            // Remove the cooldown suffix from the display name
            _label.text = (labelText.Substring(labelText.Length - 8, 8) == "Cooldown") ? labelText.Substring(0, labelText.Length - 8) : labelText;
            _label.tooltip = cooldownProperty.tooltip;
            if (cooldownProperty == null)
            {
                Debug.LogError($"null property _reference {labelText}");
                return;
            }

            _cooldownReference = cooldownProperty;
            SerializedProperty pauseProperty = cooldownProperty.FindPropertyRelative("_paused");

            // Gets the base script to run methods
            _cooldown = EditorHelper.GetTargetObjectOfProperty(_cooldownReference) as Cooldown;

            _progressReference = _cooldownReference.FindPropertyRelative("_progress");
            _bar.BindProperty(_progressReference);

            _slider.BindProperty(_progressReference);
            _slider.RegisterValueChangedCallback((change) => OnProgressChanged(change));

            _progressField.BindProperty(_progressReference);

            // Bind events
            _pauseButton.clicked += () => FlipFlopPause(pauseProperty);
            _pauseButton.clicked += () => UpdateButton(pauseProperty);
            UpdateButton(pauseProperty);

            _maxField.RegisterValueChangedCallback(change => MaxChanged(change));
            _maxField.BindProperty(_cooldownReference.FindPropertyRelative("Max"));

        }


        // Event called when the progess on the cooldown changes
        void OnProgressChanged(ChangeEvent<float> change)
        {
            //Debug.Log($"1 {change.newValue} {change.previousValue} {_progressReference.floatValue}");
            if (change.newValue == change.previousValue || change.newValue == 0) { return; }
            _progressReference.floatValue = change.newValue;
            if (_cooldown.StartTick()) { _cooldownReference.FindPropertyRelative("_ticking").boolValue = true; }
            _cooldownReference.serializedObject.ApplyModifiedProperties();
        }

        void MaxChanged(ChangeEvent<float> change)
        {
            //Debug.Log($"Max {change.newValue} {change.previousValue} {_progressReference.floatValue}");
            _bar.highValue = change.newValue;
            _slider.highValue = change.newValue;
            if (change.newValue == _cooldown.Max) { return; }
            _cooldown.StartTick();
            _cooldownReference.serializedObject.ApplyModifiedProperties();
        }

        void FlipFlopPause(SerializedProperty pausedProperty)
        {
            if (pausedProperty.boolValue)
            {
                pausedProperty.boolValue = false;
                if (_cooldown.Resume()) { _cooldownReference.FindPropertyRelative("_ticking").boolValue = true; }
            }
            else
            {
                pausedProperty.boolValue = true;
                _cooldown.Pause();
                _cooldownReference.FindPropertyRelative("_ticking").boolValue = false;
            }
            _cooldownReference.serializedObject.ApplyModifiedProperties();
        }

        void UpdateButton(SerializedProperty pausedProperty)
        {
            if (pausedProperty.boolValue)
            {
                _pauseButtonIcon.style.backgroundImage = _unpause;
            }
            else
            {
                _pauseButtonIcon.style.backgroundImage = _pause;
            }
        }
    }
}