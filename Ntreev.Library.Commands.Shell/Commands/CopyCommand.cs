using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class CopyCommand : ICommand
    {
        public string Name
        {
            get { return "copy"; }
        }

        public CommandTypes Types
        {
            get { return CommandTypes.None; }
        }

        public void Execute()
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
