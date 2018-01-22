using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class UnknownArgumentsTest : IUnknownArgument
    {
        [TestMethod]
        public void ValueTest()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("--value 0", CommandParsingTypes.OmitCommandName);
        }

        [TestMethod]
        public void ValueIncludedEqualsTest()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("--value=0", CommandParsingTypes.OmitCommandName);
        }

        bool IUnknownArgument.Parse(string key, string value)
        {
            return true;
        }
    }
}
