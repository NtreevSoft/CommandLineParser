using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 클래스 또는 메소드에 추가적으로 사용할 스위치가 정의되어 있는 static class 타입을 설정합니다.
    /// 속성의 이름을 설정하지 않을 경우에는 static class 내에 CommandSwitch 특성을 갖고 있는 public 모든 속성이 추가됩니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandStaticSwitchAttribute : Attribute
    {
        private readonly string typeName;
        private readonly Type type;
        private readonly string[] propertyNames;

        public CommandStaticSwitchAttribute(string typeName, params string[] propertyNames)
            : this(Type.GetType(typeName), propertyNames)
        {
            
        }

        public CommandStaticSwitchAttribute(Type type, params string[] propertyNames)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null && type.IsAbstract && type.IsSealed)
            {
                this.type = type;
                this.typeName = type.AssemblyQualifiedName;
            }
            else
            {
                throw new InvalidOperationException("type is not static class.");
            }
            this.propertyNames = propertyNames;
        }

        public string TypeName
        {
            get { return this.typeName; }
        }

        public string[] PropertyNames
        {
            get { return this.propertyNames; }
        }

        internal Type StaticType
        {
            get { return this.type; }
        }
    }
}
