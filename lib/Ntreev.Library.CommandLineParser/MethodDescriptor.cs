using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Ntreev.Library
{
    public class MethodDescriptor
    {
        private readonly MethodInfo methodInfo;
        private readonly CommandMethodAttribute attribute;
        //private MethodUsageProvider usageProvider;
        private readonly List<SwitchDescriptor> switches = new List<SwitchDescriptor>();

        public MethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.attribute = this.methodInfo.GetCommandMethodAttribute();

            //if (this.attribute.UsageProvider == null)
            //{
            //    this.usageProvider = new InternalMethodUsageProvider(this);
            //}
            //else
            //{
            //    this.usageProvider = TypeDescriptor.CreateInstance(
            //       null,
            //       this.attribute.UsageProvider,
            //       new Type[] { typeof(MethodDescriptor), },
            //       new object[] { this, }) as MethodUsageProvider;
            //}

            foreach (var item in methodInfo.GetParameters())
            {
                var switchDescriptor = new SwitchDescriptor(item);
                this.switches.Add(switchDescriptor);
            }

            var attr = this.methodInfo.GetCustomAttribute<CommandMethodSwitchAttribute>();
            if (attr != null)
            {
                foreach (var item in attr.PropertyNames)
                {
                    var switchDescriptor = CommandDescriptor.GetMethodSwitchDescriptors(methodInfo.DeclaringType)[item];
                    if (switchDescriptor == null)
                        throw new SwitchException(string.Format("{0} 은(는) 존재하지 않는 속성입니다.", item));
                    this.switches.Add(switchDescriptor);
                }
            }

            //if (this.attribute.Name == CommandLineInvoker.defaultMethod)
            //{
            //    if (this.switches.Count > 0)
            //        throw new SwitchException("default 메소드는 필수 또는 선택 인자를 가질 수 없습니다.");
            //}
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.attribute.Name) == true)
                    return this.methodInfo.Name;
                return this.attribute.Name;
            }
        }

        //public MethodUsageProvider UsageProvider
        //{
        //    get { return this.usageProvider; }
        //}

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        public SwitchDescriptor[] Switches
        {
            get { return this.switches.ToArray(); }
        }

        public string Description
        {
            get { return this.methodInfo.GetDescription(); }
        }

        public string Summary
        {
            get { return this.methodInfo.GetSummary(); }
        }

        internal void Invoke(object target, string arguments)
        {
            var helper = new SwitchHelper(this.switches);
            helper.Parse(target, arguments);

            var values = new ArrayList();
            var s = this.switches.ToDictionary(item => item.Name);

            foreach (var item in methodInfo.GetParameters())
            {
                var switchDescriptor = s[item.Name];

                var value = switchDescriptor.GetVaue(target);
                values.Add(value);
            }

            this.methodInfo.Invoke(target, values.ToArray());
        }
    }
}
