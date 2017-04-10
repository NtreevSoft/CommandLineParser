using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            parser.Parse("poke:4004");
        }

        class Settings
        {
            [CommandProperty]
            public int Value { get; set; }
        }
    }
}
