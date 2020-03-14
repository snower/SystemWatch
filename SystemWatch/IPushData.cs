using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemWatch
{
    interface IPushData
    {
        void PushData(DateTime now, double total,double current,double percent, object[] param);
    }
}
