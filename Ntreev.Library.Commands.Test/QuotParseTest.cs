using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class QuotParseTest
    {
        [TestMethod]
        public void SingleQuotTest()
        {
            var parser = new CommandLineParser(this);
            var text1 = "abc test 123";
            var text2 = "'abc test 123'";
            var args = string.Join(" ", "--value", text2);
            parser.Parse(args, CommandParsingTypes.OmitCommandName);

            Assert.AreEqual(text1, this.Value);
        }

        [TestMethod]
        public void SingleQuotInSingleQuotTest()
        {
            var parser = new CommandLineParser(this);
            var text1 = "abc 'test' 123";
            var text2 = "'abc \\'test\\' 123'";
            var args = string.Join(" ", "--value", text2);
            parser.Parse(args, CommandParsingTypes.OmitCommandName);

            Assert.AreEqual(text1, this.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SingleQuotInSingleQuotTest_Fail()
        {
            var parser = new CommandLineParser(this);
            var text1 = "abc 'test' 123";
            var text2 = "'abc 'test' 123'";
            var args = string.Join(" ", "--value", text2);
            parser.Parse(args, CommandParsingTypes.OmitCommandName);

            Assert.AreEqual(text1, this.Value);
        }

        [TestMethod]
        public void DoubleQuotTest()
        {
            var parser = new CommandLineParser(this);
            var text1 = "abc test 123";
            var text2 = "\"abc test 123\"";
            var args = string.Join(" ", "--value", text2);
            parser.Parse(args, CommandParsingTypes.OmitCommandName);

            Assert.AreEqual(text1, this.Value);
        }

        [TestMethod]
        public void DoubleQuotInDoubleQuotTest()
        {
            var parser = new CommandLineParser(this);
            var text1 = "abc \"test\" 123";
            var text2 = "\"abc \\\"test\\\" 123\"";
            var args = string.Join(" ", "--value", text2);
            parser.Parse(args, CommandParsingTypes.OmitCommandName);

            Assert.AreEqual(text1, this.Value);
        }

        [CommandProperty]
        public string Value
        {
            get; set;
        }
    }
}
