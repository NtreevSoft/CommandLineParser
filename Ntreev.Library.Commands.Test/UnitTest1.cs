using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var settings = new Settings();
            var parser = new CommandLineParser(settings);
            parser.ParseArguments("");
        }

        class Settings
        {
            [CommandProperty(IsImplicit = false)]
            [DefaultValue("")]
            public string List { get; set; }
        }
    }
}
