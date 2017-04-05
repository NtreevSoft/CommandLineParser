using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [Export(typeof(IShell))]
    class Shell : Terminal, IShell
    {
        private readonly CommandContextBase commandContext;
        private bool isCancellationRequested;

        [ImportingConstructor]
        public Shell(CommandContextBase commandContext)
        {
            this.commandContext = commandContext;
            this.commandContext.Name = string.Empty;
            this.Prompt = "shell";
        }

        public string Prompt
        {
            get; set;
        }

        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        public void Start()
        {
            string line;

            while ((line = this.ReadLine(this.Prompt + ">")) != null)
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
                return this.NextCompletion(commandNames, text);
                //return this.commandContext.Commands.Select(item => item.Name).OrderBy(item => item).FirstOrDefault();
            }
            else if (items.Length == 1)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
                if (commandNames.Contains(items[0]) == true)
                {
                    var command = this.commandContext.Commands[items[0]];
                    if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true && this.commandContext.IsCommandVisible(command) == true)
                    {
                        var methodNames = CommandDescriptor.GetMethodDescriptors(command).Select(item => item.Name)
                                                                                         .Where(item => item.StartsWith(find))
                                                                                         .OrderBy(item => item)
                                                                                         .ToArray();
                        return this.NextCompletion(methodNames, text);
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
                return this.PrevCompletion(commandNames, text);
                //return this.commandContext.Commands.Select(item => item.Name).OrderBy(item => item).FirstOrDefault();
            }
            else if (items.Length == 1)
            {
                var commandNames = this.commandContext.Commands.Select(item => item.Name).ToArray();
                if (commandNames.Contains(items[0]) == true)
                {
                    var command = this.commandContext.Commands[items[0]];
                    if (command.Types.HasFlag(CommandTypes.HasSubCommand) == true && this.commandContext.IsCommandVisible(command) == true)
                    {
                        var methodNames = CommandDescriptor.GetMethodDescriptors(command).Select(item => item.Name)
                                                                                         .Where(item => item.StartsWith(find))
                                                                                         .OrderBy(item => item)
                                                                                         .ToArray();
                        return this.PrevCompletion(methodNames, text);
                    }
                }
            }

            return null;
        }

        private static string[] SplitAll(string text)
        {
            var pattern = @"^((""[^""]*"")|(\S+))";
            var match = Regex.Match(text, pattern);
            var argList = new List<string>();

            while (match.Success)
            {
                text = text.Substring(match.Length).Trim();
                argList.Add(match.Value);
                match = Regex.Match(text, pattern);
            }

            return argList.ToArray();
        }

        private string NextCompletion(string[] items, string text)
        {
            items = items.OrderBy(item => item)
                         .ToArray();
            if (items.Contains(text) == true)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    var r = string.Compare(text, items[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < items.Length)
                            return items[i + 1];
                        else
                            return items.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < items.Length; i++)
                {
                    var r = string.Compare(text, items[i], true);
                    if (r < 0)
                    {
                        return items[i];
                    }
                }
            }
            return text;
        }

        private string PrevCompletion(string[] items, string text)
        {
            items = items.OrderBy(item => item)
                         .ToArray();
            if (items.Contains(text) == true)
            {
                for (var i = items.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, items[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return items[i - 1];
                        else
                            return items.Last();
                    }
                }
            }
            else
            {
                for (var i = items.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, items[i], true);
                    if (r < 0)
                    {
                        return items[i];
                    }
                }
            }
            return text;
        }
    }
}
