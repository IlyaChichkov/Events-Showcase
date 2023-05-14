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
            case "Object":
                inputField = new ObjectField(name);
                break;
            default:
                inputField = new TextField(name);
                break;
        }
        return inputField;
    }
    private MyUILibrary.ObjectTogglesView objectsView;
    private string selectedCommand = "";
    private void SetCommandParametersPopup(List<Object> targetObjects, string parametersString)
    {
        Debug.Log("> " + parametersString);
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

        objectsView = new MyUILibrary.ObjectTogglesView(choices.ToArray());
        parameters_window.Add(objectsView);

        Button exec_command_btn = new Button(ExecCommand);
        exec_command_btn.text = "Execute";
        parameters_window.Add(exec_command_btn);
    }

    private string GetParameterElementValue(VisualElement element)
    {
        string val = "";

        if (element is TextField)
        {
            TextField field = (TextField)element;
            return field.value;
        }
        if (element is IntegerField)
        {
            IntegerField field = (IntegerField)element;
            return field.value.ToString();
        }
        if (element is LongField)
        {
            LongField field = (LongField)element;
            return field.value.ToString();
        }
        if (element is DoubleField)
        {
            DoubleField field = (DoubleField)element;
            return field.value.ToString();
        }
        if (element is FloatField)
        {
            FloatField field = (FloatField)element;
            return field.value.ToString();
        }
        if (element is Vector2Field)
        {
            Vector2Field field = (Vector2Field)element;
            return field.value.ToString();
        }
        Debug.LogWarning("UES: Warning! No such parameter type implimented yet! Type: " + element.GetType());
        return val;
    }

    private void ExecCommand()
    {
        VisualElement command_setup = root.Query<VisualElement>("command-setup");
        command_setup.style.display = DisplayStyle.None;

        string inputParameters = "";

        foreach (var parameterInput in commandParametersInputs)
        {
            inputParameters += GetParameterElementValue(parameterInput) + ";";
        }
        inputParameters = inputParameters.TrimEnd(';');

        Debug.Log(inputParameters);
        string[] targets = objectsView.GetEnabledObjects();
        if (targets[0] == "all")
        {
            UES.EventManager.Instance.ExecuteCommand(selectedCommand + " all");
            return;
        }
        foreach (var item in targets)
        {
            Debug.Log(item + " " + selectedCommand);
            UES.EventManager.Instance.ExecuteCommand(selectedCommand + ";" + inputParameters, item);
        }
    }

    private void UpdateEventsList()
    {
        Debug.Log("UES: Update Events List!");
        var ESManager = UES.EventManager.Instance;
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
            VisualElement btns_container = new VisualElement();
            btns_container.style.flexDirection = FlexDirection.Row;
            btns_container.style.flexWrap = Wrap.Wrap;


            foreach (var ev in ESManager.GetEventsByGroup(group))
            {
                string command = ev.GetName();
                Button btn = new Button(delegate
                {
                    command_setup.style.display = DisplayStyle.Flex;
                    SetCommandParametersPopup(ev.GetObjets(), ev.GetCommandParameters());
                    selectedCommand = command;
                });
                //command-setup
                btn.text = command;
                btn.style.flexShrink = 0;
                btns_container.Add(btn);
            }
            fd.Add(btns_container);
            events_container.Add(fd);
        }
    }
    public void CreateGUI()
    {
        Debug.Log("CreateGUI");
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Uxml_Assets/UES_Window.uxml");
        VisualElement FromUXML = visualTree.Instantiate();
        root.Add(FromUXML);

        Button btn_reload = root.Query<Button>("btn-reload");
        btn_reload.clicked += delegate
        {
            UES.EventManager.Instance.FullEventsReload(delegate { UpdateEventsList(); });
        };

        UpdateEventsList();
    }
}