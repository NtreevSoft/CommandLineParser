using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.Commands
{
    [Export(typeof(ICommand))]
    [GitSummary("AddSummary")]
    [GitDescription("AddDescription")]
    class AddCommand : ICommand
    {
        public AddCommand()
        {
            this.Path = string.Empty;
        }

        public bool HasSubCommand
        {
            get { return false; }
        }

        public string Name
        {
            get { return "add"; }
        }

        public void Execute()
        {
            
        }

        [CommandSwitch(Required = true)]
        [GitDescription("PathDescription_AddCommand")]
        [DisplayName("<pathspec>...")]
        public string Path
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'n')]
        [Description("Don’t actually add the file(s), just show if they exist and/or will be ignored.")]
        public bool DryRun
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'v')]
        [Description("Be verbose.")]
        public bool Verbose
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'f')]
        [Description("Allow adding otherwise ignored files.")]
        public bool Force
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'i')]
        [GitDescription("InteractiveDescription_AddCommand")]
        public bool Interactive
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'P')]
        [GitDescription("PatchDescription_AddCommand")]
        public bool Patch
        {
            get;set;
        }
    }
}
