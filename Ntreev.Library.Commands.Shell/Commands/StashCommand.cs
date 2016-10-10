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
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class StashCommand : ICommand
    {
        public CommandTypes Types
        {
            get { return CommandTypes.HasSubCommand; }
        }

        public string Name
        {
            get { return "stash"; }
        }

        public void Execute()
        {

        }

        [CommandMethod("list")]
        public void List(string options)
        {


        }

        [CommandMethod("show")]
        [CommandMethodSwitch("Path", "Port")]
        public void Show(int value, int test = 0)
        {
            Console.WriteLine("value : {0}", value);
            Console.WriteLine("test : {0}", test);
        }

        [CommandMethod("save")]
        [CommandMethodSwitch("Patch", "KeepIndex", "IncludeUntracked", "All", "Quit")]
        [ShellDescription("SaveDescription_StashCommand")]
        public void Save(string message)
        {


        }

        [CommandSwitch(ShortName = 'p')]
        [ShellDescription("PatchDescription_StashCommand")]
        public bool Patch
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'k')]
        public bool KeepIndex
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'u')]
        public bool IncludeUntracked
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'a')]
        public bool All
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'q')]
        public bool Quit
        {
            get; set;
        }

        [CommandSwitch]
        public int Path
        {
            get; set;
        }

        [CommandSwitch(ShortName = 't', Required = true)]
        public int Port
        {
            get; set;
        }
    }
}
