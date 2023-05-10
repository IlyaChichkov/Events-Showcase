using System;

namespace UES
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string CommandName { get; }
        public string CommandDescription { get; }
        public string CommandGroup { get; }

        public ConsoleCommandAttribute(string commandName)
        {
            CommandName = commandName;
            CommandGroup = "default";
            CommandDescription = "";
        }
        public ConsoleCommandAttribute(string commandName, string commandDescription = "")
        {
            CommandName = commandName;
            CommandDescription = commandDescription;
            CommandGroup = "default";
        }
        public ConsoleCommandAttribute(string commandName, string commandGroup = "default", string commandDescription = "")
        {
            CommandName = commandName;
            CommandDescription = commandDescription;
            CommandGroup = commandGroup;
        }
    }
}