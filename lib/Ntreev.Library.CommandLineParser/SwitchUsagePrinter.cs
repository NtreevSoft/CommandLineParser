#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Ntreev.Library.Properties;
using System.CodeDom.Compiler;

namespace Ntreev.Library
{
    public class SwitchUsagePrinter
    {
        private readonly string name;
        private readonly string description;
        private readonly SwitchDescriptor[] switches;
        private readonly SwitchDescriptor[] options;

        public SwitchUsagePrinter(object instance, string name)
        {
            this.name = name;
            this.description = instance.GetType().GetDescription();
            var switchDescriptors = CommandDescriptor.GetSwitchDescriptors(instance);
            this.switches = switchDescriptors.Where(item => item.Required == true).ToArray();
            this.options = switchDescriptors.Where(item => item.Required == false).ToArray();
        }

        public virtual void PrintUsage(TextWriter textWriter)
        {
            using (var tw = new IndentedTextWriter(textWriter))
            {
                this.PrintUsage(tw);
            }
        }

        private void PrintUsage(IndentedTextWriter textWriter)
        {
            var args = this.switches.Aggregate("", (l, n) => l += "[" + n.Name + "] ", item => item);

            textWriter.WriteLine("{0}: {1}", Resources.Description, this.description); 
            textWriter.WriteLine("{0}: {1} {2}", Resources.Usage, this.name, args);
            textWriter.WriteLine();

            if (this.switches.Length > 0)
            {
                textWriter.WriteLine("required : ");
                textWriter.Indent++;
                var usages = new string[this.switches.Length];
                var descriptions = new string[this.switches.Length];
                for (var i = 0; i < this.switches.Length; i++)
                {
                    var item = this.switches[i];
                    usages[i] = item.UsageProvider.Usage;
                    descriptions[i] = item.UsageProvider.Description;
                }
                this.PrintUsages(textWriter, usages, descriptions);
                textWriter.Indent--;
                textWriter.WriteLine();
            }

            if (this.options.Length > 0)
            {
                textWriter.WriteLine("options : ");
                textWriter.Indent++;
                var usages = new string[this.options.Length];
                var descriptions = new string[this.options.Length];
                for (var i = 0; i < this.options.Length; i++)
                {
                    var item = this.options[i];
                    usages[i] = item.UsageProvider.Usage;
                    descriptions[i] = item.UsageProvider.Description;
                }
                this.PrintUsages(textWriter, usages, descriptions);
                textWriter.Indent--;
                textWriter.WriteLine();
            }
        }

        private void PrintUsages(TextWriter textWriter, string[] usages, string[] descriptions)
        {
            var maxLength = 0;
            foreach (var item in usages)
            {
                maxLength = Math.Max(maxLength, item.Length);
            }
            maxLength += maxLength % 4;

            for (var i = 0; i < usages.Length; i++)
            {
                var usage = usages[i].PadRight(maxLength);
                var description = descriptions[i];
                textWriter.WriteLine("{0} {1}", usage, description);
            }
        }
    }
}