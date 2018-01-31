using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Parse
{
    [CommandStaticProperty(typeof(GlobalSettings))]
    class Settings
    {
        public Settings()
        {
            this.Libraries = new string[] { };
        }

        [CommandProperty(IsRequired = true)]
        [Description("service name")]
        public string ServiceName
        {
            get; set;
        }

        [CommandProperty("path", IsRequired = true)]
        [Description("path to work")]
        public string WorkingPath
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("10001")]
        [Description("port")]
        [Browsable(true)]
        public int Port
        {
            get; set;
        }

        [CommandProperty('c')]
        [Description("use cache")]
        public bool UseCache
        {
            get; set;
        }

        [CommandProperty("cache-size", IsExplicit = true)]
        [Description("cache size. default is 1024")]
        [DefaultValue("1024")]
        public int CacheSize
        {
            get; set;
        }

        [CommandPropertyArray]
        [Description("library paths.")]
        public string[] Libraries
        {
            get; set;
        }
    }
}
