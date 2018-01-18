using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    class CustomCommandPropertyDescriptorTest : ICustomCommandPropertyDescriptor
    {
        public CustomCommandPropertyDescriptorTest()
        {

        }

        [TestMethod]
        public void Test1()
        {
            var parser = new CommandLineParser(this);
            parser.Parse("--list -c", CommandParsingTypes.OmitCommandName);
        }

        public int Value
        {
            get; set;
        }

        #region ICustomCommandPropertyDescriptor

        PropertyInfo[] ICustomCommandPropertyDescriptor.Properties
        {
            get
            {
                return new PropertyInfo[]
                {
                    this.GetType().GetProperty(nameof(Value)),
                };
            }
        }

        #endregion
    }
}
