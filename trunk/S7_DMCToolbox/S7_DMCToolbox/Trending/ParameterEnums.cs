using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trending
{
    public enum DEVICE_TYPE
    {
        PROFINET = 0,
        PROFIBUS = 1
    }

    public enum PLC_TYPE
    {
        S7300 = 1,
        S71200 = 0,
    }

    public enum AREA_TYPE
    {
        DB = 0x84,
        IN = 0x81,
        OUT = 0x82,
        MEMORY = 0x83,
    }

    public enum DATA_TYPES
    {
        TYPE_INT = 0,
        TYPE_FLOAT = 1,
        TYPE_BOOL = 2
    }

    public enum READOUT_RESOLUTION
    {
        READOUT_001   = 1,
        READOUT_0005  = 2,
        READOUT_0001  = 3,
        READOUT_00005 = 4,
    }

    public enum TAG_UNITS
    {
        IN      = 1,
        MM      = 2,
        HIDDEN  = 3,
    }

    public enum REQUEST_INTERVAL
    {
        REQUEST_5SEC    = 5000,
        REQUEST_2SEC    = 2000,
        REQUEST_1SEC    = 1000,
        REQUEST_500MSEC = 500,
        REQUEST_200MSEC = 200,
    }
}
