using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace MyUILibrary
{
    // Derives from BaseField<bool> base class. Represents a container for its input part.
    public class ObjectTogglesView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ObjectTogglesView, UxmlTraits> { }

        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
        {

        }

        public string labelAttr { get; set; }

        // In the spirit of the BEM standard, the ObjectTogglesView has its own block class and two element classes. It also
        // has a class that represents the enabled state of the toggle.
        public static readonly new string ussClassName = "objecttoggles-view";

        public List<ObjectToggle> m_TogglesList;

        // Custom controls need a default constructor. This default constructor calls the other constructor in this
        // class.
        public ObjectTogglesView() { }

        // This constructor allows users to set the contents of the label.
        public ObjectTogglesView(string[] objects)
        {
            // Style the control overall.
            AddToClassList(ussClassName);

            m_TogglesList = new List<ObjectToggle>();
            // Get the BaseField's visual input element and use it as the background of the slide.
            foreach (var objToggleName in objects)
            {
                ObjectToggle m_Toggle = new ObjectToggle(objToggleName);
                m_Toggle.SetLabel(objToggleName);
                Add(m_Toggle);
                m_TogglesList.Add(m_Toggle);
                if (objToggleName == "All")
                {
                    m_Toggle.OnChange += ToggleAll;
                }
            }
        }

        private void ToggleAll(bool value)
        {
            foreach (var m_Toggle in m_TogglesList)
            {
                if (m_Toggle.label == "All")
                {
                    continue;
                }
                m_Toggle.value = value;
            }
        }

        public string[] GetEnabledObjects()
        {
            if (m_TogglesList.Any(obj => obj.GetLabel() == "All" && obj.value == true))
            {
                return new string[] { "all" };
            }
            return m_TogglesList.Where(obj => obj.value == true && obj.GetLabel() != "All").Select(obj => obj.GetLabel()).ToArray();
        }
    }
}