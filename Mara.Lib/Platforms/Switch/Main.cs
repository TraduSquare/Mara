﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.Switch
{
    class Main
    {
        HOS horizon;

        Main(string Keys)
        {
            this.horizon = new HOS(Keys);
        }
    }
}
