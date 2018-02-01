using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class LockTest
    {
        private readonly CommandLineParser parser;

        public LockTest()
        {
            this.parser = new CommandLineParser("lock", this);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod1()
        {
            this.parser.Parse("lock");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod2()
        {
            this.parser.Parse("lock -m");
        }

        [TestMethod]
        public void TestMethod3()
        {
            this.parser.Parse("lock -m 123");
            Assert.AreEqual("", this.Path);
            Assert.AreEqual("123", this.Comment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod4()
        {
            this.parser.Parse("lock current_path");
        }

        [TestMethod]
        public void TestMethod5()
        {
            this.parser.Parse("lock current_path -m 123");
            Assert.AreEqual("current_path", this.Path);
            Assert.AreEqual("123", this.Comment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod6()
        {
            this.parser.Parse("lock current_path -m 123 -i");
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
