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

        [CommandProperty(Required = true)]
        [DisplayName("<pathspec>...")]
        public string Path
        {
            get; set;
        }

        [CommandProperty(ShortName = 'n')]
        public bool DryRun
        {
            get; set;
        }

        [CommandProperty(ShortName = 'v')]
        public bool Verbose
        {
            get; set;
        }

        [CommandProperty(ShortName = 'f')]
        public bool Force
        {
            get; set;
        }

        [CommandProperty(ShortName = 'i')]
        public bool Interactive
        {
            get; set;
        }

        [CommandProperty(ShortName = 'P')]
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
