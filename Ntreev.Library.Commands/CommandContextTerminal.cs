using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandContextTerminal : Terminal
    {
        private readonly CommandContextBase commandContext;
        private bool isCancellationRequested;
        private string prompt;
        private string prefix;
        private string postfix;

        public CommandContextTerminal(CommandContextBase commandContext)
        {
            this.commandContext = commandContext;
        }

        public string Prompt
        {
            get { return this.prompt ?? string.Empty; }
            set
            {
                this.prompt = value;
            }
        }

        public string Prefix
        {
            get { return this.prefix ?? string.Empty; }
            set
            {
                this.prefix = value;
            }
        }

        public string Postfix
        {
            get { return this.postfix ?? string.Empty; }
            set
            {
                this.postfix = value;
            }
        }

        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        public void Start()
        {
            string line;

            while ((line = this.ReadString(this.Prefix + this.Prompt + this.Postfix)) != null)
            {
                try
                {
                    this.commandContext.Execute(this.commandContext.Name + " " + line);
                }
                catch (Exception e)
                {
                    this.commandContext.Out.WriteLine(e.Message);
                }
                if (this.IsCancellationRequested == true)
                    break;
            }
        }

        public bool IsCancellationRequested
        {
            get { return this.isCancellationRequested; }
        }

        protected override string[] GetCompletion(string[] items, string find)
        {
            if (items.Length == 0)
            {
                var query = from item in this.commandContext.Commands
                            let name = item.Name
                            where name.StartsWith(find)
                            select name;
                return query.ToArray();
            }
            else if (items.Length == 1)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;
                if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                {
                    var query = from item in CommandDescriptor.GetMethodDescriptors(command)
                                let name = item.Name
                                where name.StartsWith(find)
                                select name;
                    return query.ToArray();
                }
                else
                {
                    var completions = this.GetCompletions(command, string.Empty, find);
                    if (completions != null)
                    {
                        return completions;
                    }
                }
            }
            else if (items.Length >= 2)
            {
                var commandName = items[0];
                var command = this.GetCommand(commandName);
                if (command == null)
                    return null;

                var methodName = items[1];
                var methodDescriptor = this.GetMethodDescriptor(command, methodName);
                if (methodDescriptor == null)
                    return null;

                if (methodDescriptor.Members.Length == 0)
                    return null;

                var memberList = new List<CommandMemberDescriptor>(methodDescriptor.Members);
                var argList = new List<string>(items.Skip(2));


                if(argList.Any())
                {
                    foreach (var item in memberList)
                    {
                        if (item.NamePattern == argList.Last())
                        {

                            return this.GetCompletions(command, methodDescriptor, item, find);
                        }
                    }
                }
                //var arg = string.Empty;

                for (var i = 0; i < argList.Count; i++)
                {
                    if (memberList.Any() == false)
                        break;
                    var arg = argList[i];

                    foreach(var item in memberList)
                    {
                        if(item.NamePattern == arg)
                        {
                            int qwr = 0;
                        }
                    }

                    var member = memberList.First();
                    if (member.IsRequired == true)
                        memberList.RemoveAt(0);
                    
                }

                if (memberList.Any() == true)
                {
                    return this.GetCompletions(command, methodDescriptor, memberList.First(), find);
                }

                //var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();

                //if (commandNames.Contains(commandName) == true)
                //{
                //    var command = this.commandContext.Commands[commandName];
                //    if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                //    {
                //        var methodName = items[1];
                //        if (this.commandContext.IsMethodVisible(command, methodName) == true)
                //        {
                //            var methodDescriptor = CommandDescriptor.GetMethodDescriptor(command, methodName);
                //            if (methodDescriptor.Members.Length > 0)
                //            {
                //                var completions = this.GetCompletions(command, methodDescriptor, methodDescriptor.Members[items.Length - 2], find);
                //                return completions;
                //            }
                //        }
                //    }
                //}
            }

            return null;
        }

        protected virtual string[] GetCompletions(ICommand command, string methodName, string find)
        {
            return null;
        }

        protected virtual string[] GetCompletions(ICommand command, CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            return null;
        }


        private ICommand GetCommand(string commandName)
        {
            var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
            if (commandNames.Contains(commandName) == true)
            {
                var command = this.commandContext.Commands[commandName];
                if (this.commandContext.IsCommandVisible(command) == true)
                    return command;
            }
            return null;
        }

        private CommandMethodDescriptor GetMethodDescriptor(ICommand command, string methodName)
        {
            if (command.Types.HasFlag(CommandTypes.HasSubCommand) == false)
                return null;

            if (this.commandContext.IsMethodVisible(command, methodName) == false)
                return null;
                
                    return CommandDescriptor.GetMethodDescriptor(command, methodName);
        }
    }
}
