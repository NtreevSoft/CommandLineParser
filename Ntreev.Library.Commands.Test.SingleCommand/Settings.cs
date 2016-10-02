using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test.SingleCommand
{
    [Summary("경로내의 하위 폴더 및 파일들을 표시합니다.")]
    [Description("경로내의 하위 폴더 및 파일들을 표시합니다. 폴더 및 파일의 생성시간을 표시합니다.")]
    class Settings
    {
        public Settings()
        {
            this.DirectoryName = string.Empty;
        }

        [CommandSwitch(Name = "dir", Required = true)]
        [Description("경로를 나타냅니다.")]
        public string DirectoryName
        {
            get; set;
        }

        [CommandSwitch(ShortName = 's', NameType = SwitchNameTypes.ShortName)]
        [Description("해당 경로에 하위 폴더의 내용들도 표시합니다.")]
        public bool IsRecursive
        {
            get; set;
        }

        [CommandSwitch]
        [Description("경로내의 하위 폴더 및 파일들의 이름만 표시합니다.")]
        public bool OnlyName
        {
            get; set;
        }
    }
}
