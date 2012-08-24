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
using System.ComponentModel;
using System.Net;
using Ntreev.Library;

namespace CommandLineParserTest.Options
{
    class FileInfoTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            FileInfo fileInfo = new FileInfo((string)value);
            if (fileInfo.Exists == false)
                return null;
            return fileInfo;
        }
    }

    class BaseTypeSwitches
    {
        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }

        [TypeConverter(typeof(FileInfoTypeConverter))]
        public FileInfo Path { get; set; }

        public AttributeTargets AttributeTargets { get; set; }
    }

    class RequiredSwitches
    {
        [CommandSwitch(Required=true)]
        public int Index { get; set; }

        [CommandSwitch(Required = true)]
        public string Text { get; set; }

        [CommandSwitch(Required = true)]
        public int Number { get; set; }
    }

    class ArgSeperatorSwitches
    {
        [CommandSwitch(ArgSeperator = '\0')]
        public int Level { get; set; }

        [CommandSwitch(ArgSeperator = ':')]
        public bool IsAlive { get; set; }
    }

    class DuplicatedOptions
    {
        [CommandSwitch("index")]
        public int Index { get; set; }

        [CommandSwitch("index")]
        public int Index2 { get; set; }
    }

    class ListSwitches
    {
        List<int> numbers = new List<int>();

        public ListSwitches()
        {

        }

        public List<int> Numbers { get; set; }

        public List<int> InternalNumbers
        {
            get { return this.numbers; }
        }

        public List<string> Texts { get; set; }

        [CommandParser(typeof(IPAddressListParser))]
        public List<IPAddress> IPs { get; set; }

        [CommandParser(typeof(PathListParser))]
        public List<string> PathList { get; set; }

        class IPAddressListParser : ListParser
        {
            protected override object OnItemParse(string arg, Type itemType)
            {
                return IPAddress.Parse(arg);
            }
        }

        class PathListParser : ListParser
        {
            protected override string[] SplitArgument(string arg)
            {
                return arg.Split(new char[] { '|', });
            }
        }
    }
}