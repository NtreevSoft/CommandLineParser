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
    class VersionCommand : CommandBase
    {
        private readonly CommandContextBase commandContext;

        public VersionCommand(CommandContextBase commandContext)
            : base("--version", true)
        {
            this.commandContext = commandContext;
        }

        [CommandProperty(ShortName = 'q', ShortNameOnly = true)]
        public bool IsQuiet
        {
            get; set;
        }

        protected override void OnExecute()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            using (var writer = new CommandTextWriter(this.commandContext.Out))
            {
                if (this.IsQuiet == false)
                {
                    writer.WriteLine(string.Join(" ", this.commandContext.Name, this.commandContext.Version).Trim());
                    writer.WriteLine(info.LegalCopyright);
                }
                else
                {
                    writer.WriteLine(this.commandContext.Version);
                }
            }
        }
    }
}
