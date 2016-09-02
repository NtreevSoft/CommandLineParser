using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication
{
    [Export(typeof(ICommand))]
    [Description("wqerwqer")]
    class AddCommand : ICommand
    {
        public AddCommand()
        {
            this.Path = string.Empty;
        }

        public bool HasSubCommand
        {
            get { return false; }
        }

        public string Name
        {
            get { return "add"; }
        }

        public void Execute()
        {

        }

        [CommandSwitch(Required = true)]
        [Description("추가할 파일 또는 폴더의 경로를 나타냅니다.")]
        public string Path
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'r')]
        [Description("대상이 경로일때 하위 목록들까지 추가할지에 대한 여부를 나타냅니다.")]
        public bool Recursive
        {
            get; set;
        }
    }
}
