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
