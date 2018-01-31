using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class RunTest
    {
        private readonly CommandLineParser parser;

        public RunTest()
        {
            this.parser = new CommandLineParser("run", this);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod1()
        {
            this.parser.Parse("run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod2()
        {
            this.parser.Parse("run -l");
        }

        [TestMethod]
        public void TestMethod3()
        {
            this.parser.Parse("run current_path");
            Assert.AreEqual("current_path", this.RepositoryPath);
            Assert.IsNull(this.Authentication);
        }

        [TestMethod]
        public void TestMethod4()
        {
            this.parser.Parse("run current_path -l");
            Assert.AreEqual("current_path", this.RepositoryPath);
            Assert.AreEqual("admin", this.Authentication);
        }

        [TestMethod]
        public void TestMethod5()
        {
            this.parser.Parse("run current_path -l member");
            Assert.AreEqual("current_path", this.RepositoryPath);
            Assert.AreEqual("member", this.Authentication);
        }

        [CommandProperty(IsRequired = true)]
        public string RepositoryPath
        {
            get; set;
        }

        [CommandProperty('l', IsExplicit = true)]
        [DefaultValue("admin")]
        public string Authentication
        {
            get; set;
        }
    }
}
