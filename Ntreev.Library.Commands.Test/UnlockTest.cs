using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class UnunlockTest
    {
        private readonly CommandLineParser parser;

        public UnunlockTest()
        {
            this.parser = new CommandLineParser("unlock", this);
        }

        [TestMethod]
        public void TestMethod1()
        {
            this.parser.Parse("unlock");
            Assert.AreEqual("", this.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod2()
        {
            this.parser.Parse("unlock -m");
        }

        [TestMethod]
        public void TestMethod3()
        {
            this.parser.Parse("unlock current_path");
            Assert.AreEqual("current_path", this.Path);
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string Path
        {
            get; set;
        }
    }
}
