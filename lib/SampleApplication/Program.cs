#region License
//Ntreev CommandLineParser for .Net 
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
using Ntreev.Library;
using System.IO;
using System.ComponentModel;

namespace SampleApplication
{
    class Program
    {
        static void Main()
        {
            Options options = new Options();
            CommandLineParser parser = new CommandLineParser();
            SwitchAttribute.SwitchDelimiter = '-';

            try
            {
                parser.Parse(Environment.CommandLine, options, ParsingOptions.ShortNameOnly);

                foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(options))
                {
                    if (item.IsBrowsable == false || item.IsReadOnly == true)
                        continue;

                    if (item.Converter.CanConvertFrom(typeof(string)) == false)
                        continue;

                    object value = item.GetValue(options);
                    Console.WriteLine("{0} = {1}", item.Name, value);
                }

                Environment.Exit(0);
            }
            catch (SwitchException e)
            {
                parser.PrintSwitchUsage(e.SwitchName);
                Environment.Exit(1);
            }
            catch (ArgumentException)
            {
                parser.PrintUsage();
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

      
        class Options
        {
            [Switch("n")]
            [Description("백업 여부를 설정합니다.")]
            public bool NoBackup { get; set; }

            [Switch("-Text")]
            public string Text { get; set; }

            public int Number { get; set; }

            public int Number2 { get; set; }

            public TypeCode TypeCode { get; set; }

            [Switch("a", ArgSeperator=char.MinValue)]
            public AttributeTargets AttributeTargets { get; set; }

            [TypeConverter(typeof(FileInfoTypeConverter))]
            [Description("파일 경로를 나타냅니다.")]
            [DisplayName("File Path")]
            public FileInfo Path { get; set; }

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
        }
    }
}