using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.SingleCommand
{
    class Settings
    {
        public Settings()
        {
            this.CacheSize = 1024;
            this.Libraries = new string[] { };
        }

        [CommandSwitch(Name = "path", Required = true)]
        [Description("path to work")]
        public string WorkingPath
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'c', ShortNameOnly = true)]
        [Description("use cache")]
        public bool UseCache
        {
            get; set;
        }

        [CommandSwitch(Name = "cache-size")]
        [Description("cache size. default is 1024")]
        public int CacheSize
        {
            get; set;
        }

        [CommandSwitch(Name = "libs")]
        [Description("library paths.")]
        public string[] Libraries
        {
            get; set;
        }
    }
}
