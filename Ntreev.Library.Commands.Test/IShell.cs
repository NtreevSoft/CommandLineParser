﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands.Test
{
    public interface IShell
    {
        void Cancel();

        void Start();

        string Prompt { get; set; }
    }
}
