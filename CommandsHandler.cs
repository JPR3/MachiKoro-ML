using System;

namespace MachiKoro_ML
{
    public class CommandBase
    {
        public string commandId;
        public string commandDescription;
        public string commandSyntax;
        public CommandBase(string id, string description, string syntax)
        {
            commandId = id;
            commandDescription = description;
            commandSyntax = syntax;
        }
    }
    public class Command : CommandBase
    {
        private Action command;
        public Command(string id, string description, string syntax, Action command) : base(id, description, syntax)
        {
            this.command = command;
        }
        public void Invoke()
        {
            command.Invoke();
        }

    }
    public class Command<T1> : CommandBase
    {
        private Action<T1> command;
        public Command(string id, string description, string syntax, Action<T1> command) : base(id, description, syntax)
        {
            this.command = command;
        }
        public void Invoke(T1 value1)
        {
            command.Invoke(value1);
        }
    }
    public class Command<T1, T2> : CommandBase
    {
        private Action<T1, T2> command;
        public Command(string id, string description, string syntax, Action<T1, T2> command) : base(id, description, syntax)
        {
            this.command = command;
        }
        public void Invoke(T1 value1, T2 value2)
        {
            command.Invoke(value1, value2);
        }
    }
    public class Command<T1, T2, T3> : CommandBase
    {
        private Action<T1, T2, T3> command;
        public Command(string id, string description, string syntax, Action<T1, T2, T3> command) : base(id, description, syntax)
        {
            this.command = command;
        }
        public void Invoke(T1 value1, T2 value2, T3 value3)
        {
            command.Invoke(value1, value2, value3);
        }
    }
    public class Command<T1, T2, T3, T4> : CommandBase
    {
        private Action<T1, T2, T3, T4> command;
        public Command(string id, string description, string syntax, Action<T1, T2, T3, T4> command) : base(id, description, syntax)
        {
            this.command = command;
        }
        public void Invoke(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            command.Invoke(value1, value2, value3, value4);
        }
    }
}
