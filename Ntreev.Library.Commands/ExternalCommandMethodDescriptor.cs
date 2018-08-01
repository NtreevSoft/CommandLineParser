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

namespace Ntreev.Library.Commands
{
    class ExternalCommandMethodDescriptor : CommandMethodDescriptor
    {
        private readonly object instance;
        private readonly CommandMethodDescriptor methodDescriptor;

        public ExternalCommandMethodDescriptor(object instance, CommandMethodDescriptor methodDescriptor)
            : base(methodDescriptor.MethodInfo)
        {
            this.instance = instance;
            this.methodDescriptor = methodDescriptor;
        }

        public override string DescriptorName
        {
            get { return this.methodDescriptor.DescriptorName; }
        }

        public override string Name
        {
            get { return this.methodDescriptor.Name; }
        }

        public override string DisplayName
        {
            get { return this.methodDescriptor.DisplayName; }
        }

        public override CommandMemberDescriptor[] Members
        {
            get { return this.methodDescriptor.Members; }
        }

        public override string Summary
        {
            get { return this.methodDescriptor.Summary; }
        }

        public override string Description
        {
            get { return this.methodDescriptor.Description; }
        }

        public override IEnumerable<Attribute> Attributes
        {
            get { return this.methodDescriptor.Attributes; }
        }

        public object Instance
        {
            get { return this.instance; }
        }

        protected override void OnInvoke(object instance, object[] parameters)
        {
            this.MethodInfo.Invoke(this.instance, parameters);
        }
    }
}
