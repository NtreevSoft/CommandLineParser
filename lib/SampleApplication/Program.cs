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

            try
            {
                parser.Parse(Environment.CommandLine, options);

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
