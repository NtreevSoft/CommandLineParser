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

        public CommandContextTerminal(CommandContextBase commandContext)
        {
            this.commandContext = commandContext;
            this.commandContext.Name = string.Empty;
        }

        public string Prompt
        {
            get { return this.prompt ?? string.Empty; }
            set
            {
                this.prompt = value;
            }
        }

        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        public void Start()
        {
            string line;

            while ((line = this.ReadString(this.Prompt + ">")) != null)
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

        protected override string OnNextCompletion(string[] items, string text, string find)
        {
            if (items.Length == 0)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name)
                                                               .Where(item => item.StartsWith(find))
                                                               .OrderBy(item => item)
                                                               .ToArray();
                return NextCompletion(commandNames, text);
            }
            else if (items.Length == 1)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
                if (commandNames.Contains(items[0]) == true)
                {
                    var command = this.commandContext.Commands[items[0]];
                    if (this.commandContext.IsCommandVisible(command) == true)
                    {
                        if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                        {
                            var methodNames = CommandDescriptor.GetMethodDescriptors(command).Select(item => item.Name)
                                                                                             .Where(item => item.StartsWith(find))
                                                                                             .OrderBy(item => item)
                                                                                             .ToArray();
                            return NextCompletion(methodNames, text);
                        }
                        else
                        {
                            var completions = this.GetCompletions(command);
                            if (completions != null)
                            {
                                return NextCompletion(completions, text);
                            }
                        }
                    }
                }
            }

            return null;
        }

        protected override string OnPrevCompletion(string[] items, string text, string find)
        {
            if (items.Length == 0)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name)
                                                               .Where(item => item.StartsWith(find))
                                                               .OrderBy(item => item)
                                                               .ToArray();
                return PrevCompletion(commandNames, text);
            }
            else if (items.Length == 1)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
                if (commandNames.Contains(items[0]) == true)
                {
                    var command = this.commandContext.Commands[items[0]];
                    if (this.commandContext.IsCommandVisible(command) == true)
                    {
                        if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true)
                        {
                            var methodNames = CommandDescriptor.GetMethodDescriptors(command).Select(item => item.Name)
                                                                                             .Where(item => item.StartsWith(find))
                                                                                             .OrderBy(item => item)
                                                                                             .ToArray();
                            return PrevCompletion(methodNames, text);
                        }
                        else
                        {
                            var completions = this.GetCompletions(command);
                            if (completions != null)
                            {
                                return PrevCompletion(completions, text);
                            }
                        }
                    }
                }
            }

            return null;
        }

        protected virtual string[] GetCompletions(ICommand command)
        {
            return null;
        }
    }
}
