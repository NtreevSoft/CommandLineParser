using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
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

        protected override string OnCompletion()
        {
            var ss = CommandLineParser.SplitAll(this.Text);

            if (ss.Length == 1 && this.Text != string.Empty)
            {
                var commandName = ss.First();
                var commandNames = this.commandContext.Commands.Select(item => item.Name)
                                                               .OrderBy(item => item, StringComparer.CurrentCultureIgnoreCase)
                                                               .ToArray();

                if (commandNames.Contains(commandName, StringComparer.CurrentCultureIgnoreCase) == true)
                {
                    for (var i = 0; i < commandNames.Length; i++)
                    {
                        var r = string.Compare(commandName, commandNames[i], true);
                        if (r == 0 && i+1 !=commandNames.Length)
                        {
                            return commandNames[i + 1];
                        }
                    }
                }
                else
                { 
                    for (var i =0; i < commandNames.Length; i++)
                    {
                        var r = string.Compare(commandName, commandNames[i], true);
                        if (r < 0)
                        {
                            return commandNames[i];
                        }
                    }
                }
            }

            return this.Text;
        }
    }
}
