using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace UES
{

    public class ESConsole : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private TMP_Text _tipText;

        private void Awake()
        {
            _input.onEndEdit.AddListener(delegate { RunCommand(); });
            _input.onValueChanged.AddListener(delegate { CommandHelper(); });
        }

        private void CommandHelper()
        {
            string[] commands = EventManager.Instance.GetCommands();
            string inputCommand = _input.text.Split(' ')[0];
            string tip = commands.FirstOrDefault(s => s.StartsWith(inputCommand));
            if (tip == null)
            {
                tip = "No such command";
            }
            else
            {
                _tipText.text = tip + " " + EventManager.Instance.GetCommandTip(tip);
            }
        }

        private void RunCommand()
        {
            // Debug.Log($"Search {_input.text} ...");
            EventManager.Instance.ExecuteCommand(_input.text);
        }
    }

}