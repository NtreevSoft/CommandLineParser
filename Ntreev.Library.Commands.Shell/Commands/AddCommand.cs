using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    [ShellSummary("AddSummary")]
    [ShellDescription("AddDescription")]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class AddCommand : Command
    {
        public AddCommand()
            : base("add", CommandTypes.None)
        {
            this.Path = string.Empty;
        }

        [CommandSwitch(Required = true)]
        [DisplayName("<pathspec>...")]
        public string Path
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'n')]
        public bool DryRun
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'v')]

        public bool Verbose
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'f')]
        public bool Force
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'i')]
        public bool Interactive
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'P')]
        public bool Patch
        {
            get;set;
        }
    }
}
