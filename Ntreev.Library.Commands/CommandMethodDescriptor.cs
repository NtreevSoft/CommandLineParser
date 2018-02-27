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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Ntreev.Library.Commands
{
    public abstract class CommandMethodDescriptor
    {
        private readonly MethodInfo methodInfo;

        protected CommandMethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        public abstract string DescriptorName
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract string DisplayName
        {
            get;
        }

        public abstract CommandMemberDescriptor[] Members
        {
            get;
        }

        public abstract string Summary
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract IEnumerable<Attribute> Attributes
        {
            get;
        }

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        protected abstract void OnInvoke(object instance, object[] parameters);

        internal void Invoke(object instance, string arguments, IEnumerable<CommandMemberDescriptor> descriptors, bool init)
        {
            var parser = new ParseDescriptor(typeof(CommandParameterDescriptor), descriptors, arguments, init);
            parser.SetValue(instance);

            var values = new ArrayList();
            var nameToDescriptors = descriptors.ToDictionary(item => item.DescriptorName);

            foreach (var item in this.methodInfo.GetParameters())
            {
                var descriptor = nameToDescriptors[item.Name];
                var value = descriptor.GetValueInternal(instance);
                values.Add(value);
            }
            this.OnInvoke(instance, values.ToArray());
        }
    }

    
}
