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
using System.ComponentModel;

namespace Ntreev.Library.CommandLineParser
{
    static class CommandSwitchDescriptorContext
    {
        static readonly public Parser DefaultBooleanParser = new BooleanParser();
        static readonly public Parser DefaultParser = new Parser();
        static readonly public Parser DefaultListParser = new ListParser();

        delegate bool Filter(CommandSwitchAttribute obj);

        static bool ByRequired(CommandSwitchAttribute obj)
        {
            return obj.Required == true;
        }

        static int NameComparison(CommandSwitchDescriptor x, CommandSwitchDescriptor y)
        {
            return x.Name.CompareTo(y.Name);
        }

        static int ShortNameComparison(CommandSwitchDescriptor x, CommandSwitchDescriptor y)
        {
            return x.ShortName.CompareTo(y.ShortName);
        }

        static CommandSwitchDescriptorCollection GetSwitches(object instance, CommandSwitchAttributeCollection extenedSwitchAttributes, Filter filter)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
            List<CommandSwitchDescriptor> validProperties = new List<CommandSwitchDescriptor>();

            foreach (PropertyDescriptor item in properties)
            {
                if (item.IsBrowsable == false)
                    continue;

                if (item.IsReadOnly == true)
                {
                    if (item.PropertyType.IsValueType == true)
                        continue;
                }

                Parser parser = GetParser(item, instance);

                if (parser == null)
                    continue;

                CommandSwitchAttribute optionAttribute = item.Attributes[typeof(CommandSwitchAttribute)] as CommandSwitchAttribute;

                if (optionAttribute == null)
                {
                    if (extenedSwitchAttributes != null && extenedSwitchAttributes.ContainsKey(item.Name) == true)
                        optionAttribute = extenedSwitchAttributes[item.Name];
                    else
                        optionAttribute = CommandSwitchAttribute.DefaultValue;
                }

                if (filter == null || filter(optionAttribute) == true)
                    validProperties.Add(new CommandSwitchDescriptor(item, optionAttribute));
            }
            return new CommandSwitchDescriptorCollection(validProperties);
        }

        public static CommandSwitchDescriptorCollection GetSwitches(object instance, CommandSwitchAttributeCollection extenedSwitchAttributes)
        {
            return GetSwitches(instance, extenedSwitchAttributes, (Filter)null);
        }

        public static CommandSwitchDescriptorCollection GetRequiredSwitches(object instance, CommandSwitchAttributeCollection extenedSwitchAttributes)
        {
            return GetSwitches(instance, extenedSwitchAttributes, CommandSwitchDescriptorContext.ByRequired);
        }

        public static Parser GetParser(PropertyDescriptor propertyDescriptor, object instance)
        {
            ParserAttribute parserAttribute = propertyDescriptor.Attributes[typeof(ParserAttribute)] as ParserAttribute;

            if (parserAttribute != null)
            {
                return TypeDescriptor.CreateInstance(null, parserAttribute.ParserType, null, null) as Parser;
            }

            object value = propertyDescriptor.GetValue(instance);

            if (propertyDescriptor.PropertyType.IsArray == true || typeof(System.Collections.IList).IsAssignableFrom(propertyDescriptor.PropertyType) == true)
            {
                return CommandSwitchDescriptorContext.DefaultListParser;
            }
            else if (propertyDescriptor.PropertyType == typeof(bool))
            {
                return CommandSwitchDescriptorContext.DefaultBooleanParser;
            }
            else if (propertyDescriptor.PropertyType.IsValueType == true)
            {
                return CommandSwitchDescriptorContext.DefaultParser;
            }

            if(propertyDescriptor.Converter.CanConvertFrom(typeof(string)) == true)
                return CommandSwitchDescriptorContext.DefaultParser;
            return null;
        }
    }
}