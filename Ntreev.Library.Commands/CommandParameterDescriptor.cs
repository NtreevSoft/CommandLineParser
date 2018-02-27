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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public sealed class CommandParameterDescriptor : CommandMemberDescriptor
    {
        private readonly ParameterInfo parameterInfo;
        private readonly string summary;
        private readonly string description;
        private object value;

        public CommandParameterDescriptor(ParameterInfo parameterInfo)
            : base(new CommandPropertyAttribute() { IsRequired = true }, parameterInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(parameterInfo.Member.DeclaringType);
            var paramAttr = parameterInfo.GetCustomAttribute<ParamArrayAttribute>();
            this.parameterInfo = parameterInfo;
            this.summary = provider.GetSummary(parameterInfo);
            this.description = provider.GetDescription(parameterInfo);
            this.value = this.parameterInfo.DefaultValue;
        }

        public override string DisplayName
        {
            get
            {
                var displayName = this.parameterInfo.GetDisplayName();
                if (displayName != string.Empty)
                    return displayName;
                return this.Name;
            }
        }

        public override string Summary
        {
            get { return this.summary; }
        }

        public override string Description
        {
            get { return this.description; }
        }

        public override object DefaultValue
        {
            get { return this.parameterInfo.DefaultValue; }
        }

        public override Type MemberType
        {
            get { return this.parameterInfo.ParameterType; }
        }

        public override IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach (Attribute item in this.parameterInfo.GetCustomAttributes(true))
                {
                    yield return item;
                }
            }
        }

        protected override void SetValue(object instance, object value)
        {
            this.value = value;
        }

        protected override object GetValue(object instance)
        {
            return this.value;
        }
    }
}
