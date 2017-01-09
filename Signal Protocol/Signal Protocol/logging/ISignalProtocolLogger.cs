using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal_Protocol.logging
{
    interface ISignalProtocolLogger
    {
        void log(int priority, String tag, String message);
    }
}
