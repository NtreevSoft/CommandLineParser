
namespace Ntreev.Library
{
    public abstract class MethodUsageProvider
    {
        readonly MethodDescriptor methodDescriptor;

        /// <summary>
        /// <seealso cref="MethodDescriptor"/>를 사용하여 <seealso cref="MethodUsageProvider"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="methodDescriptor">
        /// 스위치의 정보를 담고 있는 <seealso cref="MethodDescriptor"/>의 인스턴스입니다.
        /// </param>
        public MethodUsageProvider(MethodDescriptor methodDescriptor)
        {
            this.methodDescriptor = methodDescriptor;
        }

        /// <summary>
        /// 스위치의 정보를 담고 있는 <seealso cref="MethodDescriptor"/>의 인스턴스를 가져옵니다.
        /// </summary>
        protected MethodDescriptor MethodDescriptor
        {
            get { return this.methodDescriptor; }
        }

        /// <summary>
        /// 기본적인 사용방법을 가져옵니다.
        /// </summary>
        public abstract string Usage
        {
            get;
        }
    }
}
