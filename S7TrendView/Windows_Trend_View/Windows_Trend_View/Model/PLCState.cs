using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windows_Trend_View
{
    /// <summary>
    /// Enum defining PLC States
    /// </summary>
    public enum PLCState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
    }
}
