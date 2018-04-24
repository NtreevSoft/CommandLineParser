//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Ntreev.Library.Commands.Test
{
    [TestClass]
    public class RunScriptTest
    {
        private readonly CommandLineParser parser;

        public RunScriptTest()
        {
            this.parser = new CommandLineParser("run", this);

            // run --filename path
            // run "log(1);"
            // run --list
        }

        [TestMethod]
        public void TestMethod1()
        {
            this.parser.Parse("run --filename \"C:\\script.js\"");
        }

        [TestMethod]
        public void TestMethod2()
        {
            this.parser.Parse("run log(1);");
        }

        [TestMethod]
        public void TestMethod3()
        {
            this.parser.Parse("run --list");
        }

        [TestMethod]
        public void TestMethod4()
        {
            this.parser.Parse("run -l");
        }

        [TestMethod]
        public void TestMethod4_With_Args()
        {
            this.parser.Parse("run -l -- db=string port=number async=boolean");
        }

        [TestMethod]
        public void TestMethod5()
        {
            this.parser.Parse("run log(1); arg1=1 arg2=text");
        }

        [CommandProperty(IsRequired = true)]
        [CommandPropertyTrigger(nameof(Filename), "")]
        [CommandPropertyTrigger(nameof(List), false)]
        [DefaultValue("")]
        public string Script
        {
            get; set;
        }

        [CommandProperty()]
        [DefaultValue("")]
        [CommandPropertyTrigger(nameof(Script), "")]
        [CommandPropertyTrigger(nameof(List), false)]
        public string Filename
        {
            get; set;
        }

        [CommandProperty("list", 'l')]
        [CommandPropertyTrigger(nameof(Script), "")]
        [CommandPropertyTrigger(nameof(Filename), "")]
        [DefaultValue(false)]
        public bool List
        {
            get; set;
        }

        [CommandPropertyArray]
        public string[] Arguments
        {
            get;
            set;
        }
    }
}
