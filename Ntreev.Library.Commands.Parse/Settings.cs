using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Parse
{
    [CommandStaticSwitch(typeof(GlobalSettings))]
    class Settings
    {
        public Settings()
        {
            this.Libraries = new string[] { };
        }

        [CommandSwitch(Required = true)]
        [Description("service name")]
        public string ServiceName
        {
            get; set;
        }

        [CommandSwitch(Name = "path", Required = true)]
        [Description("path to work")]
        public string WorkingPath
        {
            get; set;
        }

        [CommandSwitch(Required = true)]
        [DefaultValue("10001")]
        [Description("port")]
        [Browsable(true)]
        public int Port
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
        [DefaultValue("1024")]
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
