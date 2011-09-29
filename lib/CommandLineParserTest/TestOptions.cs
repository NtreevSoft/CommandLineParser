using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ntreev.Library;
using System.ComponentModel;

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
        [Switch(Required=true)]
        public int Index { get; set; }

        [Switch(Required = true)]
        public string Text { get; set; }

        [Switch(Required = true)]
        public int Number { get; set; }
    }

    class ArgSeperatorSwitches
    {
        [Switch(ArgSeperator = char.MinValue)]
        public int Level { get; set; }

        [Switch(ArgSeperator = ':')]
        public bool IsAlive { get; set; }
    }

    class DuplicatedOptions
    {
        [Switch("index")]
        public int Index { get; set; }

        [Switch("index")]
        public int Index2 { get; set; }
    }
}
