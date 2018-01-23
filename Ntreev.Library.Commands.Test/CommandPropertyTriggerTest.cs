using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class CommandPropertyTriggerTest
    {
        [TestMethod]
        public void Test()
        {
            var d1 = decimal.Parse("0.43");
            decimal i = 0.435345m;
            var ii = (int)i;
        }

        [CommandProperty(IsRequired = true)]
        public int Value1
        {
            get; set;
        }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(Value1), 1)]
        public int Value2
        {
            get; set;
        }
    }
}
