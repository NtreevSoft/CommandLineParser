using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class CommandPropertyArrayTest
    {
        [TestMethod]
        public void Test1()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("get database=a port=123 userid=abc password=1234 comment=\"connect database to \\\"a\\\"\"", CommandParsingTypes.OmitCommandName);

            CommandStringUtility.ArgumentsToDictionary(this.Arguments);
        }

        [TestMethod]
        public void Test2()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("get \"database=a b c\"", CommandParsingTypes.OmitCommandName);

            CommandStringUtility.ArgumentsToDictionary(this.Arguments);
        }

        [TestMethod]
        public void Test3()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("get \"database=\\\"a b c\\\"\"", CommandParsingTypes.OmitCommandName);

            CommandStringUtility.ArgumentsToDictionary(this.Arguments);
        }

        [TestMethod]
        public void ValueIncludedEqualsTest()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("--value=0", CommandParsingTypes.OmitCommandName);
        }


        [CommandProperty(IsRequired = true)]
        public string Command
        {
            get; set;
        }

        [CommandPropertyArray]
        public string[] Arguments
        {
            get; set;
        }
    }
}
