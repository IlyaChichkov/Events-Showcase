using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

namespace UES
{
    public class ESJson
    {
        [System.Serializable]
        private class SerializedEvents
        {
            public SerializedEvent[] events;
            public SerializedEvents(SerializedEvent[] _events)
            {
                events = _events;
            }
        }
        [System.Serializable]
        private class SerializedEvent
        {
            public string name, group, description, methodType, parameters;
            public string[] parametersNames;
            public string[] parametersTypes;
            public SerializedEvent(Event ev)
            {
                name = ev.GetName();
                group = ev.GetGroup();
                description = ev.description;
                methodType = ev.method.DeclaringType.ToString();
                parameters = ev.GetCommandParameters();
                var m_Parameters = ev.method.GetParameters();
                parametersNames = new string[m_Parameters.Length];
                parametersTypes = new string[m_Parameters.Length];
                for (int i = 0; i < m_Parameters.Length; i++)
                {
                    parametersNames[i] = m_Parameters[i].Name;
                    parametersTypes[i] = m_Parameters[i].ParameterType.ToString();
                }
            }
        }
        private static string EventsPath = Application.dataPath + "/events.json";
        public static void SaveEventsJSON(List<Event> events)
        {
            SerializedEvent[] s_Evs = new SerializedEvent[events.Count];
            for (int i = 0; i < events.Count; i++)
            {
                s_Evs[i] = new SerializedEvent(events[i]);
            }
            string json = JsonUtility.ToJson(new SerializedEvents(s_Evs));
            Debug.Log("UES: Save Completed.");
            WriteData(json);
        }
        public static async Task SaveEventsAsyncJSON(List<Event> events)
        {
            SerializedEvent[] s_Evs = new SerializedEvent[events.Count];
            for (int i = 0; i < events.Count; i++)
            {
                s_Evs[i] = new SerializedEvent(events[i]);
            }
            string json = JsonUtility.ToJson(new SerializedEvents(s_Evs));
            Debug.Log("UES: Async Save Completed.");
            WriteData(json);
        }

        public static List<Event> LoadEventsJSON()
        {
            List<Event> events = new List<Event>();
            string jsonData = ReadData();
            SerializedEvents serializedEvents = JsonUtility.FromJson<SerializedEvents>(jsonData);
            foreach (SerializedEvent s_Ev in serializedEvents.events)
            {
                Event ev = new Event(s_Ev.name, s_Ev.group, s_Ev.description, s_Ev.parameters);
                var dictionary = new Dictionary<string, string>();
                for (int index = 0; index < s_Ev.parametersNames.Length; index++)
                {
                    dictionary.Add(s_Ev.parametersNames[index], s_Ev.parametersTypes[index]);
                }
                ev.SetMethodParameters(dictionary);
                events.Add(ev);
            }
            return events;
        }

        private static void WriteData(string data)
        {
            using (StreamWriter sw = new StreamWriter(EventsPath))
            {
                sw.Write(data);
            }
        }
        private static string ReadData()
        {
            string json = "";
            using (StreamReader sr = new StreamReader(EventsPath))
            {
                json = sr.ReadToEnd();
            }
            return json;
        }
    }

}