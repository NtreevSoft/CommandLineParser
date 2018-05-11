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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        internal CommandCollection()
        {

        }

        public bool Contains(string commandName)
        {
            if (this.commands.ContainsKey(commandName) == false)
                return false;
            return true;
        }

        public ICommand this[string commandName]
        {
            get
            {
                if (this.commands.ContainsKey(commandName) == false)
                    return null;
                return this.commands[commandName];
            }
        }

        public int Count
        {
            get { return this.commands.Count; }
        }

        internal void Add(ICommand command)
        {
            if (this.commands.ContainsKey(command.Name) == true)
                throw new ArgumentException($"command '{command.Name}' is already registered.");
            this.commands.Add(command.Name, command);
        }

        #region IEnumerable

        IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
        {
            foreach (var item in this.commands)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.commands)
            {
                yield return item.Value;
            }
        }

        #endregion
    }
}
