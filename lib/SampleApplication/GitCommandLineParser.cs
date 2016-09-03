using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication
{
    class GitCommandLineParser : CommandLineParser
    {
        public GitCommandLineParser(object instance)
            : base(instance)
        {
        }

        protected override SwitchUsagePrinter CreateUsagePrinterCore(string name, object instance)
        {
            return new GitSwitchUsagePrinter(name, instance);
        }
    }
}
