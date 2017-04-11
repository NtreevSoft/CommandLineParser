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
    [CommandStaticProperty(typeof(GlobalSettings))]
    class AddCommand : CommandBase
    {
        public AddCommand()
            : base("add")
        {
            this.Path = string.Empty;
        }

        [CommandProperty(IsRequired = true)]
        [DisplayName("<pathspec>...")]
        public string Path
        {
            get; set;
        }

        [CommandProperty('n')]
        public bool DryRun
        {
            get; set;
        }

        [CommandProperty('v')]
        public bool Verbose
        {
            get; set;
        }

        [CommandProperty('f')]
        public bool Force
        {
            get; set;
        }

        [CommandProperty('i')]
        public bool Interactive
        {
            get; set;
        }

        [CommandProperty('P')]
        public bool Patch
        {
            get;set;
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }
    }
}
