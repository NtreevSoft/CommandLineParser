using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication
{
    [Export(typeof(ICommand))]
    class StashCommand : ICommand
    {
        public bool HasSubCommand
        {
            get { return true; }
        }

        public string Name
        {
            get { return "stash"; }
        }

        public void Execute()
        {

        }

        [CommandMethod("save")]
        public void Save(int value)
        {


        }
    }
}
