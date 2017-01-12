using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Parse
{
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    static class GlobalSettings
    {
        [CommandProperty]
        public static string ID
        {
            get; set;
        }

        [CommandProperty]
        public static string Password
        {
            get; set;
        }
    }
}
