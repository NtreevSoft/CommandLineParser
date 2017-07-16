#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Ntreev.Library.Commands.Shell.Properties;
using System.Resources;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Ntreev.Library.Commands.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            var ssss = "----------------------------------------------------------------------------------------";
            var text = "ショット判定をパンヤに正確に合わせれば、より強力なスーパー パンヤが出ることもあります。 ";

            //Console.WriteLine(ssss);
            //Console.WriteLine(text);

            Terminal.Init();
            //var llll = Terminal.GetLength(text);
            Console.Write("\r");
             var tableData = new TableDataBuilder("taiwan", "korean", "japan");
            tableData.Add("เปิดใช้งาน Super PANGYA ที่เสริมพลังเมื่อแถบการตีหยุดตรงเขต PANGYA", "샷 판정을 팡야에 정확하게 맞히는 경우 성능이 향상된 슈퍼팡야가 나오기도 합니다.", text);

            Console.Out.Print(tableData);

            return;
            var sss = Console.OutputEncoding.GetBytes("เ최");
            Console.Write("เ");
            for (var i = 0; i < 40; i++)
            {
                Console.WriteLine(i);
            }
            var shell = Container.GetService<IShell>();
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);

                //Console.Write(string.Empty.PadRight(Console.BufferWidth + 2, 'c'));

                while (true)
                {
                    //if (Console.BufferWidth == 81)
                    {
                        //Console.Write(string.Empty.PadRight(Console.BufferWidth + 2, 'c'));
                        //break;
                    }
                    System.Threading.Thread.Sleep(1000);
                    //break;
                }
                //    //Console.WriteLine(DateTime.Now);
                //    System.Threading.Thread.Sleep(1000);
                //}
                //System.Threading.Thread.Sleep(10000);
                //Console.Write(DateTime.Now);
            });

            shell.Prompt = Directory.GetCurrentDirectory();
            shell.Start();
        }
    }
}