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
        private MethodUsageProvider usageProvider;
        private readonly Dictionary<string, SwitchDescriptor> nameToStwich = new Dictionary<string, SwitchDescriptor>();

        public MethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.attribute = this.methodInfo.GetCommandMethodAttribute();

            if (this.attribute.UsageProvider == null)
            {
                this.usageProvider = new InternalMethodUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                   null,
                   this.attribute.UsageProvider,
                   new Type[] { typeof(MethodDescriptor), },
                   new object[] { this, }) as MethodUsageProvider;
            }

            foreach (ParameterInfo item in methodInfo.GetParameters())
            {
                SwitchDescriptor switchDescriptor = new SwitchDescriptor(item);
                nameToStwich.Add(switchDescriptor.Name, switchDescriptor);
            }

            List<SwitchDescriptor> switches = new List<SwitchDescriptor>(this.nameToStwich.Values);

            foreach (CommandMethodSwitchAttribute item in this.methodInfo.GetCustomAttributes(typeof(CommandMethodSwitchAttribute), true))
            {
                SwitchDescriptor switchDescriptor = CommandDescriptor.GetSwitchDescriptors(methodInfo.DeclaringType)[item.PropertyName];
                nameToStwich.Add(switchDescriptor.Name, switchDescriptor);
            }
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

        public MethodUsageProvider UsageProvider
        {
            get { return this.usageProvider; }
        }

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        public SwitchDescriptor[] Switches
        {
            get { return this.nameToStwich.Values.ToArray(); }
        }

        public string Description
        {
            get { return this.methodInfo.GetDescription(); }
        }

        internal void Invoke(object instance, string[] parameters)
        {
            SwitchHelper helper = new SwitchHelper(this.nameToStwich.Values);
            helper.Parse(instance, parameters, ParseOptions.None);

            ArrayList values = new ArrayList();
            foreach (ParameterInfo item in methodInfo.GetParameters())
            {
                SwitchDescriptor switchDescriptor = this.nameToStwich[item.Name];

                object value;
                if (switchDescriptor == null)
                {
                    if (item.TryGetDefaultValue(out value) == false)
                        throw new Exception("매개 변수의 갯수가 적습니다");
                }
                else
                {
                    value = switchDescriptor.GetVaue(instance);
                }
                values.Add(value);
            }

            this.methodInfo.Invoke(instance, values.ToArray());
        }

        internal void Invoke2(object instance, string[] parameters)
        {
            ArrayList values = new ArrayList();
            ParameterInfo[] infoes = methodInfo.GetParameters();
            if (parameters.Length > infoes.Length)
                throw new Exception("매개 변수의 갯수가 많습니다.");


            for (int i = 0; i < infoes.Length; i++)
            {
                ParameterInfo parameterInfo = infoes[i];
                object value;
                if (i >= parameters.Length)
                {
                    if (parameterInfo.TryGetDefaultValue(out value) == false)
                        throw new Exception("매개 변수의 갯수가 적습니다");
                }
                else
                {
                    value = parameterInfo.GetValue(parameters[i]);
                }

                values.Add(value);
            }

            this.methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
