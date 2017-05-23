using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override void OnInvoke(object instance, object[] parameters)
        {
            this.MethodInfo.Invoke(this.instance, parameters);
        }
    }
}
