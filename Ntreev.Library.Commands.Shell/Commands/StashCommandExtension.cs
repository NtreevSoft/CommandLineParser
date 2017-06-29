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
    [Export(typeof(ICommandProvider))]
    class StashCommandExtension : CommandProviderBase
    {
        public StashCommandExtension()
            : base("stash")
        {

        }

        [CommandMethod("list")]
        [CommandMethodStaticProperty(typeof(GlobalSettings))]
        public void List(string options)
        {

        }
    }
}
