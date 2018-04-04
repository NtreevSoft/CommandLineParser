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

using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Shell.Commands
{
    [Export(typeof(ICommand))]
    [UsageDescriptionProvider(typeof(ResourceUsageDescriptionProvider))]
    class StashCommand : CommandMethodBase
    {
        public StashCommand()
            : base("stash")
        {

        }

        //[CommandMethod("list")]
        //[CommandStaticProperty(typeof(GlobalSettings))]
        //public void List(string options)
        //{
        //
        //}

        [CommandMethod("show")]
        [CommandMethodProperty("Path", "Port")]
        public void Show(int value, int test = 0)
        {
            Console.WriteLine("value : {0}", value);
            Console.WriteLine("test : {0}", test);
        }

        [CommandMethod("save")]
        [CommandMethodProperty("Patch", "KeepIndex", "IncludeUntracked", "All", "Quit")]
        [ShellDescription("SaveDescription_StashCommand")]
        public void Save(string message)
        {

        }

        [CommandProperty('p')]
        [ShellDescription("PatchDescription_StashCommand")]
        public bool Patch
        {
            get; set;
        }

        [CommandProperty('k', true)]
        public bool KeepIndex
        {
            get; set;
        }

        [CommandProperty('u', true)]
        public bool IncludeUntracked
        {
            get; set;
        }

        [CommandProperty('a', true)]
        public bool All
        {
            get; set;
        }

        [CommandProperty('q', true)]
        public bool Quit
        {
            get; set;
        }

        [CommandProperty]
        public int Path
        {
            get; set;
        }

        [CommandProperty('t', IsRequired = true)]
        public int Port
        {
            get; set;
        }
    }
}
