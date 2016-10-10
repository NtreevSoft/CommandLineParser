using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell
{
    [Export(typeof(IShell))]
    class Shell : Mono.Terminal.LineEditor, IShell
    {
        private readonly CommandContext commandContext;
        private bool isCancellationRequested;

        [ImportingConstructor]
        public Shell(CommandContext commandContext)
            : base("shell")
        {
            this.commandContext = commandContext;
            this.commandContext.Name = string.Empty;
            this.HeuristicsMode = "csharp";
            this.AutoCompleteEvent += delegate (string a, int pos)
            {
                string prefix = "";
                //var completions = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
                var completions = new string[] { };
                return new Mono.Terminal.LineEditor.Completion(prefix, completions);
            };
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

            while ((line = this.Edit(this.Prompt + "> ", "")) != null)
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
    }
}
