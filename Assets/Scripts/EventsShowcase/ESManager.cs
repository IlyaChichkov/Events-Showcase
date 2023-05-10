using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UES;

class EventsRegister
{
    /*
    static public List<UES.Event> RegisterObjectCommands(object target)
    {
        List<UES.Event> _events = new List<UES.Event>();
        var type = target.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            Debug.Log($"> {method.Name}");
            var attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attribute != null)
            {
                var commandName = attribute.CommandName;
                if (_events.FindIndex(ev => ev.GetName() == commandName) >= 0)
                {
                    Debug.Log($"Already exist.");
                    // обработка конфликта имен команд
                    continue;
                }
                _events.Add(new UES.Event(commandName, method));
            }
        }
        return _events;
    }*/

    static public List<UES.Event> RegisterCommands()
    {
        List<UES.Event> _events = new List<UES.Event>();
        Debug.Log($"Register Commands!");

        var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(object).IsAssignableFrom(p));

        var methods = types
                      .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                      .Where(m => m.GetCustomAttributes(typeof(ConsoleCommandAttribute), false).Length > 0)
                      .ToArray();

        Debug.Log("> Methods:");
        foreach (var method in methods)
        {
            Debug.Log(method.Name);
            var attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attribute != null)
            {
                string commandName = attribute.CommandName;
                string commandGroup = attribute.CommandGroup;
                string commandDescription = attribute.CommandDescription;

                if (_events.FindIndex(ev => ev.GetName() == commandName) >= 0)
                {
                    Debug.Log($"Already exist.");
                    continue;
                }
                UES.Event ev = new UES.Event(commandName, method, commandGroup, commandDescription);

                Type methodType = method.DeclaringType;
                if (methodType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    UnityEngine.Object[] objects = MonoBehaviour.FindObjectsOfType(methodType);
                    ev.SetObjects(objects);
                }
                Debug.Log($"Add: {commandName} Type: {methodType}");
                _events.Add(ev);
            }
        }
        return _events;
    }

}

namespace UES
{
    public class Event
    {
        private string _name;
        private string _group;
        public string description;
        private List<UnityEngine.Object> _objects;
        public MethodInfo method;
        private string command_parameters = "";

        public Event(string name, MethodInfo method, string group = "default", string description = "")
        {
            _name = name;
            _group = group;
            this.method = method;
            this.description = description;
            SetMethodParameters();
        }

        public void AddObject(UnityEngine.Object obj)
        {
            _objects.Add(obj);
        }

        public void SetObjects(UnityEngine.Object[] objects)
        {
            _objects = objects.ToList();
        }

        public string GetName()
        {
            return _name;
        }
        public string GetGroup()
        {
            return _group;
        }
        public string GetCommandParameters()
        {
            return command_parameters;
        }

        public List<UnityEngine.Object> GetObjets()
        {
            return _objects;
        }

        private void SetMethodParameters()
        {
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                string name = parameter.Name;
                if (name[0] == '_')
                {
                    name.Remove(0, 1);
                }
                command_parameters += ($"{parameter.Name}:{Utils.GetParameterTypeName(parameter.ParameterType)};");
            }
        }
    }

    public class EventManager
    {
        private static readonly EventManager instance = new EventManager();

        public static EventManager Instance
        {
            get
            {
                return instance;
            }
        }

        private List<Event> _events = new List<Event>();

        private EventManager()
        {
            LoadEvents();
        }

        private void ShowEvents()
        {
            foreach (var ev in _events)
            {
                Debug.Log(ev.GetName());
            }
        }

        public void LoadEvents()
        {
            _events = new List<Event>(EventsRegister.RegisterCommands());
        }
        public void ExecuteCommand(string commandString)
        {
            Debug.Log($"UES: {commandString}");
            if (commandString == "rst")
            {
                Debug.Log($"UES: Reset project events.");
                LoadEvents();
                return;
            }

            var parts = commandString.Split(' ');
            var commandName = parts[0];
            Event ev = _events.Find(ev => ev.GetName() == commandName);
            if (ev == null)
            {
                Debug.LogWarning("UES: No such command!");
                // обработка неверной команды
                return;
            }
            var method = ev.method;

            var parameters = method.GetParameters();
            if (parameters.Length != parts.Length - 2)
            {
                Debug.LogWarning("UES: Wrong number of parameters!");
                // обработка неверного числа параметров
                return;
            }

            var arguments = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var argumentString = parts[i + 2];
                var argument = Convert.ChangeType(argumentString, parameterType);
                arguments[i] = argument;
            }

            UnityEngine.Object target = null;
            if (method.IsStatic)
            {
                method.Invoke(target, arguments);
                return;
            }
            if (parts[1] == "all")
            {
                foreach (UnityEngine.Object obj in ev.GetObjets())
                {
                    method.Invoke(obj, arguments);
                }
            }
            else
            {
                target = ev.GetObjets().FirstOrDefault(obj => obj.name == parts[1]);
                if (target)
                {
                    method.Invoke(target, arguments);
                }
                else
                {
                    Debug.LogWarning("UES: No object with such name to apply!");
                }
            }
        }

        public string[] GetCommands()
        {
            return _events.Select(ev => ev.GetName()).ToArray();
        }

        public Event[] GetEvents()
        {
            return _events.ToArray();
        }
        public Event[] GetEventsByGroup(string group)
        {
            return _events.Where(ev => ev.GetGroup() == group).ToArray();
        }

        public string[] GetGroups()
        {
            return _events.Select(ev => ev.GetGroup()).Distinct().ToArray();
        }


        public Event? GetCommandEvent(string command)
        {
            return _events.FirstOrDefault(ev => ev.GetName() == command);
        }

        public string GetCommandTip(string command)
        {
            string prmts = GetCommandParameters(command) ?? "Null";
            return $"[all/name] [ {prmts}]";
        }

        public string[] GetCommandObjectsNames(string command)
        {
            return GetCommandEvent(command).GetObjets().Select(obj => obj.name).ToArray();
        }

        public string GetCommandParameters(string command)
        {
            return GetCommandEvent(command)?.GetCommandParameters();
        }
    }

    class MyTest
    {
        [ConsoleCommand("set")]
        public void Hello()
        {
            Debug.Log("Hello");
        }


        [ConsoleCommand("bye")]
        public void Bye()
        {
            Debug.Log("Bye");
        }
    }
}

/*

public class ConsoleManager {
    private readonly Dictionary<string, Action> _commands = new Dictionary<string, Action>();

    public void RegisterCommands(object target) {
        var type = target.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (var method in methods) {
            var attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attribute != null) {
                var commandName = attribute.CommandName;
                var action = (Action)Delegate.CreateDelegate(typeof(Action), target, method);
                _commands.Add(commandName, action);
            }
        }
    }

    public void ExecuteCommand(string commandName) {
        if (_commands.TryGetValue(commandName, out var action)) {
            action();
        } else {
            // обработка неверной команды
        }
    }
}*/