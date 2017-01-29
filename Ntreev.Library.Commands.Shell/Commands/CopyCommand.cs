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
    class CopyCommand : CommandBase
    {
        public CopyCommand()
            : base("copy")
        {

        }

        [CommandProperty(Required = true)]
        public string SourcePath
        {
            get; set;
        }

        [CommandProperty(Required = true)]
        public string TargetPath
        {
            get; set;
        }

        [CommandShortProperty('o')]
        public bool OverWrite
        {
            get; set;
        }

        protected override void OnExecute()
        {
            File.Copy(this.SourcePath, this.TargetPath, this.OverWrite);
        }
    }
}
