using System;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Test
{
    public class LockTest
    {
        private readonly CommandLineParser parser;

        public LockTest()
        {
            this.parser = new CommandLineParser("lock", this);
        }

        public void TestMethod6()
        {
            this.parser.Parse("lock -i -m wow");
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string Path
        {
            get; set;
        }

        [CommandProperty('m')]
        [CommandPropertyTrigger(nameof(Information), false)]
        [DefaultValue("")]
        public string Comment
        {
            get; set;
        }

        [CommandProperty('i')]
        [CommandPropertyTrigger(nameof(Comment), "")]
        public bool Information
        {
            get; set;
        }

        [CommandProperty("format")]
        [CommandPropertyTrigger(nameof(Information), true)]
        [DefaultValue("xml")]
        public string FormatType
        {
            get; set;
        }
    }
}
