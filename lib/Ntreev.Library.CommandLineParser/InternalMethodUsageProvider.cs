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
                StringBuilder sb = new StringBuilder();
                MethodInfo methodInfo = this.MethodDescriptor.MethodInfo;
                sb.Append(this.MethodDescriptor.Name);

                sb.Append(" ");
                sb.Append(this.MethodDescriptor.Description);


                //foreach (ParameterInfo item in methodInfo.GetParameters())
                //{
                //    sb.AppendFormat(" ({0} as {1})", item.Name, item.ParameterType.GetSimpleName());
                //}

                return sb.ToString();
             }
        }
    }
}
