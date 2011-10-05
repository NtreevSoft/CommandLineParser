#region License
//Ntreev CommandLineParser for .Net 1.0.4295.27782
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

namespace Ntreev.Library
{
    public partial class CommandLineParser
    {
        /// <summary>
        /// 사용방법을 설정할 수 있는 방법을 제공합니다.
        /// </summary>
        public class UsagePrinter
        {
            CommandLineParser commandLineParser;
            string location;

            string GetDefaultUsage(SwitchDescriptor[] switchDescriptors)
            {
                FileInfo fileInfo = new FileInfo(this.location);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(string.Format("{0}", fileInfo.Name));

                foreach (SwitchDescriptor item in switchDescriptors)
                {
                    if (item.Required == false)
                        continue;
                    stringBuilder.Append(" " + item.UsageProvider.Usage);
                }

                stringBuilder.Append(" [options | ...]");

                return stringBuilder.ToString();
            }

            public UsagePrinter(CommandLineParser commandLineParser)
            {
                Assembly assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                this.location = assembly.Location;

                object[] attributes = null;
                attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length == 1)
                {
                    this.Title = ((AssemblyTitleAttribute)attributes[0]).Title;
                }

                attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 1)
                {
                    this.Description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }

                attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 1)
                {
                    this.Company = ((AssemblyCompanyAttribute)attributes[0]).Company;
                }

                attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 1)
                {
                    this.Copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }

                attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 1)
                {
                    this.Product = ((AssemblyProductAttribute)attributes[0]).Product;
                }


                this.Version = assembly.GetName().Version.ToString();

                this.License = Properties.Resources.License;

                this.commandLineParser = commandLineParser;
            }

            internal void PrintSwitchUsage(string switchName)
            {
                object instance = this.commandLineParser.Instance;
                if (instance == null)
                    throw new Exception();
                SwitchAttributeCollection switchAttributes = this.commandLineParser.SwitchAttributes;

                SwitchDescriptorCollection switchDescriptorCollection = SwitchDescriptorContext.GetSwitches(instance, switchAttributes);
                OnPrintSwitchUsage(switchName, switchDescriptorCollection[switchName], this.commandLineParser.TextWriter);
            }

            internal void PrintUsage()
            {
                object instance = this.commandLineParser.Instance;
                if (instance == null)
                    throw new Exception();
                SwitchAttributeCollection switchAttributes = this.commandLineParser.SwitchAttributes;

                SwitchDescriptorCollection switchDescriptorCollection = SwitchDescriptorContext.GetSwitches(instance, switchAttributes);
                OnPrintUsage(switchDescriptorCollection.ToArray(), this.commandLineParser.TextWriter);
            }

            public string Title { get; set; }

            public string Description { get; set; }

            public string Company { get; set; }

            public string Copyright { get; set; }

            public string Product { get; set; }

            public string Version { get; set; }

            public string License { get; set; }

            public string Usage { get; set; }

            protected virtual void OnPrintUsage(SwitchDescriptor[] switchDescriptors, TextWriter textWriter)
            {
                textWriter.WriteLine("{0} {1}", this.Title, this.Version);
                textWriter.WriteLine(this.Copyright);
                textWriter.WriteLine(this.Description);
                textWriter.WriteLine(this.License);
                textWriter.WriteLine();
                if (this.Usage == null)
                    textWriter.WriteLine("{0}: {1}", Properties.Resources.Usage, GetDefaultUsage(switchDescriptors));
                else
                    textWriter.WriteLine("{0}: {1}", Properties.Resources.Usage, this.Usage);

                int len = 0;

                foreach (SwitchDescriptor item in switchDescriptors)
                {
                    UsageProvider usageProvider = item.UsageProvider;

                    len = Math.Max(len, usageProvider.Usage.Length);
                }

                foreach (SwitchDescriptor item in switchDescriptors)
                {
                    UsageProvider usageProvider = item.UsageProvider;
                    string usage = string.Format("    {0} {1}", usageProvider.Usage, usageProvider.Description);

                    textWriter.WriteLine(usage);

                    if (usageProvider.ArgumentTypeDescription != string.Empty)
                    {
                        textWriter.WriteLine("        {0}", usageProvider.ArgumentTypeDescription);
                    }
                }
            }

            protected virtual void OnPrintSwitchUsage(string switchName, SwitchDescriptor switchDescriptor, TextWriter textWriter)
            {
                if (switchDescriptor == null)
                {
                    textWriter.WriteLine("{0} is invalid switch", switchName);
                }
                else
                {
                    UsageProvider usageProvider = switchDescriptor.UsageProvider;
                    textWriter.WriteLine("Invalid usage");
                    textWriter.WriteLine("{0} {1}", usageProvider.Usage, usageProvider.Description);
                }
            }
        }
    }
}