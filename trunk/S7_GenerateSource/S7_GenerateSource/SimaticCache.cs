using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S7_GenerateSource
{
    public class SimaticCache
    {

        public class SimaticBlocksCache
        {
            public string strProgramLogPath;
            public Dictionary<int, string> dicBlocksLogPath = new Dictionary<int,string>();
        }

        public Dictionary<int, SimaticBlocksCache> dicProgramsLogPath = new Dictionary<int, SimaticBlocksCache>();
    }
}
