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
    class StashCommandPartial : SubCommandBase
    {
        public StashCommandPartial()
            : base("stash")
        {

        }

        [CommandMethod("list")]
        [CommandStaticProperty(typeof(GlobalSettings))]
        public void List(string options)
        {

        }
    }
}
