using _S.Editor.UIToolkitExtras;
using _S.Editor.UXMLElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(UXMLRemmappingBinder.InputActionBinder))]
public class InputActionBinderDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var inputName = property.FindPropertyRelative("_inputName");
        var actionProperty = property.FindPropertyRelative("_actionReference");
        var extendedProperty = property.FindPropertyRelative("_sectionExtended");
        var root = new SectionElement(inputName.stringValue ,extendedProperty);

        var nameField = new PropertyField(inputName);
        nameField.RegisterValueChangeCallback(x =>
        {
            root.label.text = x.changedProperty.stringValue;
        });
        root.LinkedAddContent(nameField);

        var actionField = new PropertyField(actionProperty);
        root.LinkedAddContent(actionField);
        root.LinkedAddContent(new InputBindElement(actionField, property.FindPropertyRelative("_primary")))
        .LinkedAddContent(new InputBindElement(actionField, property.FindPropertyRelative("_secondary")))
        .LinkedAddContent(new InputBindElement(actionField, property.FindPropertyRelative("_controller")));

        return root;
    }
    class InputBindElement : VisualElement
    {
        SerializedProperty _actionProperty;
        SerializedProperty _bindingIdProperty;
        SerializedProperty _bindingIndexProperty;
        const string _disableBind = "Disable Bind";
        private string[] _BindingOptionIDs;
        public InputBindElement(PropertyField actionField, SerializedProperty property)
        {
            _actionProperty = property.FindPropertyRelative("_actionReference");
            _bindingIdProperty = property.FindPropertyRelative("_bindingID");
            _bindingIndexProperty = property.FindPropertyRelative("_bindingIndex");
            var aboveProperty = property.FindPreviousProperty().FindPropertyRelative("_actionReference");
            _actionProperty.objectReferenceValue = aboveProperty.objectReferenceValue;
            property.serializedObject.ApplyModifiedProperties();

            VisualElement root = this;

            //Setup popup values
            FindBindingValues(out List<string> bindings, out string currentBinding);

            var popup = new PopupField<string>($"{property.displayName} Binding");
            popup.style.marginTop = 3;
            popup.choices = bindings;
            popup.value = currentBinding;
            popup.formatSelectedValueCallback =
                (x) =>
                {
                    if (x != "")
                    {
                        if (x == _disableBind)
                        {
                            _bindingIdProperty.stringValue = "";
                            _bindingIndexProperty.intValue = -1;
                            property.serializedObject.ApplyModifiedProperties();
                            return x;
                        }
                        int index = popup.choices.IndexOf(x) - 1;
                        if (index < 0)
                        {
                            return x;
                        }
                        _bindingIdProperty.stringValue = _BindingOptionIDs[index];
                        _bindingIndexProperty.intValue = index;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    return x;
                };
            popup.formatListItemCallback =
                (x) =>
                {
                    if (x == "skip")
                    {
                        return null;
                    }
                    return x;
    };

            actionField.RegisterValueChangeCallback(x =>
            {
                _actionProperty.objectReferenceValue = x.changedProperty.objectReferenceValue;
                property.serializedObject.ApplyModifiedProperties();
                if (x.changedProperty.objectReferenceValue == null)
                {
                    popup.style.display = DisplayStyle.None;
                }
                else
                {
                    FindBindingValues(out List<string> bindings, out string currentBinding);
                    popup.choices = bindings;
                    popup.value = currentBinding;
                    popup.style.display = DisplayStyle.Flex;
                }
            });
            if (bindings == null)
            {
                popup.style.display = DisplayStyle.None;
            }

            root.Add(popup);
        }

        void FindBindingValues(out List<string> bindingsList, out string selectedBindingOption)
        {
            selectedBindingOption = "";
            bindingsList = new List<string>();
            var actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
            var action = actionReference?.action;

            if (action == null)
            {
                bindingsList.Add("");
                _BindingOptionIDs = new string[0];
                return;
            }
            bindingsList.Add(_disableBind);
            selectedBindingOption = _disableBind;

            var bindings = action.bindings;
            var bindingCount = bindings.Count;
            _BindingOptionIDs = new string[bindings.Count];
            var currentBindingId = _bindingIdProperty.stringValue;
            for (var i = 0; i < bindingCount; ++i)
            {
                var binding = bindings[i];

                if (binding.isComposite)
                {
                    bindingsList.Add("skip");
                    continue;
                }
                var bindingId = binding.id.ToString();
                var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                var displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                // Create display string.
                var displayString = action.GetBindingDisplayString(i, displayOptions);
                if (displayString == "")
                {
                    displayString = $"Unbound {i}";
                }

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        var controlSchemes = string.Join(", ",
                            binding.groups.Split(InputBinding.Separator)
                                .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                // Registers the binding into the bindings selector list and ID array
                bindingsList.Add(displayString);
                _BindingOptionIDs[i] = bindingId;

                if (currentBindingId == bindingId)
                    selectedBindingOption = displayString;
            }
        }
    
    }
}
