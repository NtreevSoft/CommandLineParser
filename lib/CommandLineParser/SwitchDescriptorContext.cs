#region License
//Ntreev CommandLineParser for .Net 1.0.4461.33698
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

namespace Ntreev.Library
{
    static class SwitchDescriptorContext
    {
        static readonly public Parser DefaultBooleanParser = new BooleanParser();
        static readonly public Parser DefaultParser = new Parser();
        static readonly public Parser DefaultListParser = new ListParser();

        delegate bool Filter(SwitchAttribute obj);

        static bool ByRequired(SwitchAttribute obj)
        {
            return obj.Required == true;
        }

        static int NameComparison(SwitchDescriptor x, SwitchDescriptor y)
        {
            return x.Name.CompareTo(y.Name);
        }

        static int ShortNameComparison(SwitchDescriptor x, SwitchDescriptor y)
        {
            return x.ShortName.CompareTo(y.ShortName);
        }

        static SwitchDescriptorCollection GetSwitches(object instance, SwitchAttributeCollection extenedSwitchAttributes, Filter filter)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
            List<SwitchDescriptor> validProperties = new List<SwitchDescriptor>();

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

                SwitchAttribute optionAttribute = item.Attributes[typeof(SwitchAttribute)] as SwitchAttribute;

                if (optionAttribute == null)
                {
                    if (extenedSwitchAttributes != null && extenedSwitchAttributes.ContainsKey(item.Name) == true)
                        optionAttribute = extenedSwitchAttributes[item.Name];
                    else
                        optionAttribute = SwitchAttribute.DefaultValue;
                }

                if (filter == null || filter(optionAttribute) == true)
                    validProperties.Add(new SwitchDescriptor(item, optionAttribute));
            }
            return new SwitchDescriptorCollection(validProperties);
        }

        public static SwitchDescriptorCollection GetSwitches(object instance, SwitchAttributeCollection extenedSwitchAttributes)
        {
            return GetSwitches(instance, extenedSwitchAttributes, (Filter)null);
        }

        public static SwitchDescriptorCollection GetRequiredSwitches(object instance, SwitchAttributeCollection extenedSwitchAttributes)
        {
            return GetSwitches(instance, extenedSwitchAttributes, SwitchDescriptorContext.ByRequired);
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
                return SwitchDescriptorContext.DefaultListParser;
            }
            else if (propertyDescriptor.PropertyType == typeof(bool))
            {
                return SwitchDescriptorContext.DefaultBooleanParser;
            }
            else if (propertyDescriptor.PropertyType.IsValueType == true)
            {
                return SwitchDescriptorContext.DefaultParser;
            }

            if(propertyDescriptor.Converter.CanConvertFrom(typeof(string)) == true)
                return SwitchDescriptorContext.DefaultParser;
            return null;
        }
    }
}