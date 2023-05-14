using UnityEngine;
using UnityEngine.UIElements;

namespace MyUILibrary
{
    // Derives from BaseField<bool> base class. Represents a container for its input part.
    public class ObjectToggle : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<ObjectToggle, UxmlTraits> { }

        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
        {
            UxmlStringAttributeDescription m_Label_Attr = new UxmlStringAttributeDescription { name = "label-attr", defaultValue = "object-name" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as ObjectToggle;

                ate.Clear();

                // Get the BaseField's visual input element and use it as the background of the slide.
                ate.m_Input = new VisualElement();
                ate.m_Input.AddToClassList(inputUssClassName);
                ate.Add(ate.m_Input);

                // Create a "knob" child element for the background to represent the actual slide of the toggle.
                ate.m_Knob = new();
                ate.m_Knob.AddToClassList(inputKnobUssClassName);
                ate.m_Input.Add(ate.m_Knob);


                ate.labelAttr = m_Label_Attr.GetValueFromBag(bag, cc);
                ate.m_Label = new Label(ate.labelAttr);
                ate.m_Label.AddToClassList(labelUssClassName);
                ate.m_Input.Add(ate.m_Label);

                ate.value = false;
                ate.m_Input.EnableInClassList(inputCheckedUssClassName, ate.value);
            }
        }

        public string labelAttr { get; set; }

        public delegate void ToggleChange(bool toggle);
        public ToggleChange? OnChange;

        // In the spirit of the BEM standard, the ObjectToggle has its own block class and two element classes. It also
        // has a class that represents the enabled state of the toggle.
        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly new string labelUssClassName = "slide-toggle__label";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";

        public VisualElement m_Input;
        public VisualElement m_Knob;
        public VisualElement m_Label;

        public void SetLabel(string label)
        {
            ((Label)m_Label).text = label;
        }

        public string GetLabel()
        {
            return ((Label)m_Label).text;
        }
        // Custom controls need a default constructor. This default constructor calls the other constructor in this
        // class.
        public ObjectToggle() : this(null) { }

        // This constructor allows users to set the contents of the label.
        public ObjectToggle(string label) : base("", null)
        {
            // Style the control overall.
            AddToClassList(ussClassName);

            // Get the BaseField's visual input element and use it as the background of the slide.
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClassName);
            Add(m_Input);

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            m_Knob = new();
            m_Knob.AddToClassList(inputKnobUssClassName);
            m_Input.Add(m_Knob);

            m_Label = new Label(label);
            m_Label.AddToClassList(labelUssClassName);
            m_Input.Add(m_Label);

            // There are three main ways to activate or deactivate the ObjectToggle. All three event handlers use the
            // static function pattern described in the Custom control best practices.

            // ClickEvent fires when a sequence of pointer down and pointer up actions occurs.
            RegisterCallback<ClickEvent>(evt => OnClick(evt));
            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));
            // NavigationSubmitEvent detects input from keyboards, gamepads, or other devices at runtime.
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));
        }

        static void OnClick(ClickEvent evt)
        {
            var ObjectToggle = evt.currentTarget as ObjectToggle;
            ObjectToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnSubmit(NavigationSubmitEvent evt)
        {
            var ObjectToggle = evt.currentTarget as ObjectToggle;
            ObjectToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var ObjectToggle = evt.currentTarget as ObjectToggle;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            if (ObjectToggle.panel?.contextType == ContextType.Player)
                return;

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                ObjectToggle.ToggleValue();
                evt.StopPropagation();
            }
        }


        // All three callbacks call this method.
        void ToggleValue()
        {
            value = !value;
        }

        // Because ToggleValue() sets the value property, the BaseField class dispatches a ChangeEvent. This results in a
        // call to SetValueWithoutNotify(). This example uses it to style the toggle based on whether it's currently
        // enabled.
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            //This line of code styles the input element to look enabled or disabled.
            m_Input.EnableInClassList(inputCheckedUssClassName, newValue);

            OnChange?.Invoke(newValue);
        }
    }
}