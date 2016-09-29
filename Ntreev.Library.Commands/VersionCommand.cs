using Ntreev.Library;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class VersionCommand : ICommand
    {
        private readonly CommandContext commandContext;

        public VersionCommand(CommandContext commandContext)
        {
            this.commandContext = commandContext;
        }

        public void Execute()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            using (var writer = new CommandTextWriter(this.commandContext.Out))
            {
                if (this.IsQuiet == false)
                {
                    writer.WriteLine("{0} {1}", this.commandContext.Name, this.commandContext.Version);
                    writer.WriteLine(info.LegalCopyright);
                }
                else
                {
                    writer.WriteLine(this.commandContext.Version);
                }
            }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.AllowEmptyArgument; }
        }

        public string Name
        {
            get { return "--version"; }
        }

        [CommandSwitch(ShortName = 'q', NameType = SwitchNameTypes.ShortName)]
        [Description("버전만 표시합니다.")]
        public bool IsQuiet
        {
            get; set;
        }

        private void PrintList(CommandTextWriter writer)
        {
            this.commandContext.Parsers[this].PrintUsage();

            writer.WriteLine("AvaliableCommands");
            writer.Indent++;
            foreach (var item in this.commandContext.Parsers)
            {
                var instance = item.Value.Instance;
                var summary = instance.GetType().GetSummary();
                if (item.Key == this)
                    continue;
                writer.WriteLine(item.Key);
                writer.Indent++;
                writer.WriteMultiline(summary);
                writer.Indent--;
            }
            writer.Indent--;
            writer.WriteLine();
        }
    }
}
