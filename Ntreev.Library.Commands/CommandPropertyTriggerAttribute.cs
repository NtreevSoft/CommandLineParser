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
using Ntreev.Library.Commands.Properties;
using System.Text.RegularExpressions;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandPropertyTriggerAttribute : Attribute
    {
        private Type type;
        private string typeName;
        private string propertyName;
        private object value;

        public CommandPropertyTriggerAttribute(Type type, string propertyName, object value)
        {
            this.Validate(type, propertyName);
            this.type = type;
            this.propertyName = propertyName;
            this.value = value;
        }

        private void Validate(Type type, string propertyName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            var prop = type.GetProperty(propertyName);
            if (prop == null)
                throw new ArgumentException(propertyName);
            if (prop.GetCustomAttribute<CommandPropertyAttribute>() == null)
                throw new ArgumentException(propertyName);
        }
    }
}
