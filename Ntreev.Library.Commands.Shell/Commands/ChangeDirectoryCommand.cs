//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class ChangeDirectoryCommand : CommandBase
    {
        [Import]
        private Lazy<IShell> shell = null;
        [Import]
        private Lazy<ShellCommandContext> commandContext = null;

        public ChangeDirectoryCommand()
            : base("cd")
        {
            this.DirectoryName = string.Empty;
        }

        [CommandProperty("dir", IsRequired = true)]
        [DefaultValue("")]
        public string DirectoryName
        {
            get; set;
        }

        public TextWriter Out
        {
            get { return this.commandContext.Value.Out; }
        }

        protected override void OnExecute()
        {
            var shell = this.shell.Value;
            if (this.DirectoryName == string.Empty)
            {
                this.Out.WriteLine(shell.Prompt);
            }
            else if (this.DirectoryName == "..")
            {
                var dir = Path.GetDirectoryName(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(dir);
                shell.Prompt = dir;
            }
            else if (Directory.Exists(this.DirectoryName) == true)
            {
                var dir = new DirectoryInfo(this.DirectoryName).FullName;
                Directory.SetCurrentDirectory(dir);
                shell.Prompt = dir;
            }
            else
            {
                throw new DirectoryNotFoundException(string.Format("'{0}'은(는) 존재하지 않는 경로입니다.", this.DirectoryName));
            }
        }
    }
}
