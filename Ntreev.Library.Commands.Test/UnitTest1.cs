﻿using System;
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
            parser.Parse("--list -c", CommandParsingTypes.OmitCommandName);

            parser.Parse("--list wer -c", CommandParsingTypes.OmitCommandName);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var commands = new Commands();
            var parser = new CommandLineParser(commands);
            parser.Invoke("test a -m wow", CommandParsingTypes.OmitCommandName);
        }

        class Settings
        {
            [CommandProperty(IsImplicit = true)]
            [DefaultValue("")]
            public string List { get; set; }

            [CommandProperty('c')]
            public bool IsCancel { get; set; }

            [CommandProperty]
            [DefaultValue(5005)]
            public int Port { get; set; }
        }

        class Commands
        {
            [CommandMethod]
            [CommandMethodProperty(nameof(Message))]
            public void Test(string target1, string target2 = null)
            {
                Assert.AreEqual("a", target1);
                Assert.AreEqual(null, target2);
                Assert.AreEqual("wow", this.Message);
            }

            [CommandProperty('m', IsRequired = true)]
            public string Message
            {
                get; set;
            }
        }
    }
}
