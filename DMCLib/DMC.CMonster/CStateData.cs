using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMC.CMonster
{
    public class CStateData
    {
        public string State { get; set; }
        public List<CEventData> EventActions { get; set; }
    }
}
