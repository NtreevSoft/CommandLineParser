using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// SubCommand로 사용할 클래스에 추가로 사용될 CommandMethodAttribute가 정의되어 있는 static class 타입을 설정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandStaticMethodAttribute : Attribute
    {
        private readonly string typeName;
        private readonly Type type;
        private readonly string[] methodNames;

        public CommandStaticMethodAttribute(string typeName, params string[] methodNames)
            : this(Type.GetType(typeName), methodNames)
        {

        }

        public CommandStaticMethodAttribute(Type type, params string[] methodNames)
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
        }

        public string TypeName
        {
            get { return this.typeName; }
        }

        public string[] MethodNames
        {
            get { return this.methodNames; }
        }

        internal Type StaticType
        {
            get { return this.type; }
        }
    }
}
