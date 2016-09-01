using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ntreev.Library
{
    class InternalMethodUsageProvider : MethodUsageProvider
    {
        public InternalMethodUsageProvider(MethodDescriptor methodDescriptor)
            : base(methodDescriptor)
        {

        }

        public override string Usage
        {
            get 
            {
                var sb = new StringBuilder();
                var methodInfo = this.MethodDescriptor.MethodInfo;
                sb.Append(this.MethodDescriptor.Name);

                sb.Append(" ");
                sb.Append(this.MethodDescriptor.Description);

                return sb.ToString();
             }
        }
    }
}
