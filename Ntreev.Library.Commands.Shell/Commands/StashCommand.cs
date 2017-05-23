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
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class StashCommand : CommandMethodBase
    {
        public StashCommand()
            : base("stash")
        {

        }

        //[CommandMethod("list")]
        //[CommandStaticProperty(typeof(GlobalSettings))]
        //public void List(string options)
        //{
        //
        //}

        [CommandMethod("show")]
        [CommandMethodProperty("Path", "Port")]
        public void Show(int value, int test = 0)
        {
            Console.WriteLine("value : {0}", value);
            Console.WriteLine("test : {0}", test);
        }

        [CommandMethod("save")]
        [CommandMethodProperty("Patch", "KeepIndex", "IncludeUntracked", "All", "Quit")]
        [ShellDescription("SaveDescription_StashCommand")]
        public void Save(string message)
        {

        }

        [CommandProperty('p')]
        [ShellDescription("PatchDescription_StashCommand")]
        public bool Patch
        {
            get; set;
        }

        [CommandProperty('k')]
        public bool KeepIndex
        {
            get; set;
        }

        [CommandProperty('u', true)]
        public bool IncludeUntracked
        {
            get; set;
        }

        [CommandProperty('a')]
        public bool All
        {
            get; set;
        }

        [CommandProperty('q')]
        public bool Quit
        {
            get; set;
        }

        [CommandProperty]
        public int Path
        {
            get; set;
        }

        [CommandProperty('t', IsRequired = true)]
        public int Port
        {
            get; set;
        }
    }
}
