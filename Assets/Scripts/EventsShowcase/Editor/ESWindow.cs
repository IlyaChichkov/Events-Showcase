using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ESWindow : EditorWindow
{
    static ESWindow window;
    public static VisualElement root;

    private List<VisualElement> commandParametersInputs;

    [MenuItem("Window/UES")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (ESWindow)EditorWindow.GetWindow(typeof(ESWindow));
        window.titleContent = new GUIContent("Events Showcase 2");
        window.Show();
        Debug.Log("Show");
    }

    private VisualElement CreateParameterInputField(string parameter)
    {
        string name = parameter.Split(':')[0];
        string typeString = parameter.Split(':')[1];
        VisualElement inputField;
        Debug.Log("Name: " + name);
        Debug.Log("Type: " + typeString);
        switch (typeString)
        {
            case "string":
                inputField = new TextField(name);
                break;
            case "int":
                inputField = new IntegerField(name);
                break;
            case "long":
                inputField = new LongField(name);
                break;
            case "double":
                inputField = new DoubleField(name);
                break;
            case "float":
                inputField = new FloatField(name);
                break;
            case "bool":
                inputField = new Toggle(name);
                break;
            case "decimal":
                inputField = new DoubleField(name);
                break;
            case "Vector2":
                inputField = new Vector2Field(name);
                break;
            case "Vector2Int":
                inputField = new Vector2IntField(name);
                break;
            case "Vector3":
                inputField = new Vector3Field(name);
                break;
            case "Vector3Int":
                inputField = new Vector3IntField(name);
                break;
            default:
                inputField = new TextField(name);
                break;
        }
        return inputField;
    }

    private void SetCommandParametersPopup(List<Object> targetObjects, string parametersString)
    {
        var parameters = parametersString.Split(';');

        VisualElement parameters_window = root.Query<VisualElement>("command-parameters");
        parameters_window.Clear();

        List<string> choices = new List<string>() { "All" };
        commandParametersInputs = new List<VisualElement>();

        foreach (var parameter in parameters)
        {
            if (parameter == "") continue;
            VisualElement inputField = CreateParameterInputField(parameter);
            commandParametersInputs.Add(inputField);
            parameters_window.Add(inputField);
        }
        if (targetObjects != null)
        {
            foreach (var targetObject in targetObjects)
            {
                choices.Add(targetObject.name);
            }
        }
        DropdownField objectChoose = new DropdownField("Choose objects:", choices, 0);
        parameters_window.Add(objectChoose);
    }

    public void CreateGUI()
    {
        var ESManager = UES.EventManager.Instance;
        Debug.Log("CreateGUI");
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UES_Window.uxml");
        VisualElement FromUXML = visualTree.Instantiate();
        root.Add(FromUXML);

        Button btn_reload = root.Query<Button>("btn-reload");
        btn_reload.clicked += ESManager.LoadEvents;

        VisualElement events_container = root.Query<VisualElement>("events-container");
        events_container.Clear();

        VisualElement command_setup = root.Query<VisualElement>("command-setup");
        command_setup.style.display = DisplayStyle.None;

        List<VisualElement> close_command_setup = root.Query(className: "close-setup-popup").ToList();
        foreach (var item in close_command_setup)
        {
            item.RegisterCallback<ClickEvent>(delegate
            {
                command_setup.style.display = DisplayStyle.None;
            });
        }

        foreach (var group in ESManager.GetGroups())
        {
            Foldout fd = new Foldout();
            fd.text = group;

            foreach (var ev in ESManager.GetEventsByGroup(group))
            {
                string command = ev.GetName();
                Button btn = new Button(delegate
                {
                    command_setup.style.display = DisplayStyle.Flex;
                    SetCommandParametersPopup(ev.GetObjets(), ev.GetCommandParameters());
                });
                //command-setup
                btn.text = command;
                fd.Add(btn);
            }
            events_container.Add(fd);
        }

    }
}