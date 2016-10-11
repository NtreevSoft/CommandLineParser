using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class CopyCommand : Command
    {
        public CopyCommand()
            : base("copy")
        {

        }

        public override void Execute()
        {
            File.Copy(this.SourcePath, this.TargetPath, this.OverWrite);
        }

        [CommandSwitch(Required = true)]
        public string SourcePath
        {
            get; set;
        }

        [CommandSwitch(Required = true)]
        public string TargetPath
        {
            get; set;
        }

        [CommandShortSwitch('o')]
        public bool OverWrite
        {
            get; set;
        }
    }
}
