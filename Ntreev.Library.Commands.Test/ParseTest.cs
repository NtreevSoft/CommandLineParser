using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class ParseTest
    {
        private readonly CommandLineParser parser;

        public ParseTest()
        {
            this.parser = new CommandLineParser("parse", this);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod1()
        {
            this.parser.Parse("parse --boolean false");
            Assert.IsTrue(this.Boolean);
        }

        [CommandProperty]
        public bool Boolean
        {
            get; set;
        }

        [CommandProperty]
        public int Number
        {
            get; set;
        }

        [CommandProperty]
        public string String
        {
            get; set;
        }
    }
}
