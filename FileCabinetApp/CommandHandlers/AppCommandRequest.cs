﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class AppCommandRequest
    {
        public string Command { get; set; }

        public string Parameters { get; set; }
    }
}
