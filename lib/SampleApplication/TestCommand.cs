using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication
{
    [Export(typeof(ICommand))]
    class TestCommand : ICommand
    {
        public bool HasSubCommand
        {
            get { return false; }
        }

        public string Name
        {
            get { return "test"; }
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        [CommandSwitch(ShortName = 'v')]
        public bool IsVisible
        {
            get; set;
        }
    }
}
