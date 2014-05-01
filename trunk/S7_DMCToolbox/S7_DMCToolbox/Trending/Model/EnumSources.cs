using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows_Display_App;

namespace Trending
{
    public class EnumSources : NotifyPropertyChangedBase
    {
        public List<EnumComboItem<TAG_UNITS>> DisplayUnitsList { get { return _TagUnitsList; } }
        public List<EnumComboItem<PLC_TYPE>> PLCTypeList { get { return _PLCTypeList; } }
        public List<EnumComboItem<AREA_TYPE>> AreaTypeList {get { return _AreaTypeList; }} 
        public List<EnumComboItem<REQUEST_INTERVAL>> RequestIntervalList { get { return _RequestIntervalList; } }
        public List<EnumComboItem<READOUT_RESOLUTION>> ReadoutResolutionListMM { get { return _ReadoutResolutionListMM; } }
        public List<EnumComboItem<READOUT_RESOLUTION>> ReadoutResolutionListIN { get { return _ReadoutResolutionListIN; } }

        // Build lists. 
        private readonly List<EnumComboItem<TAG_UNITS>> _TagUnitsList = new List<EnumComboItem<TAG_UNITS>>()
        {
            new EnumComboItem<TAG_UNITS>(){Content = "IN", Value = TAG_UNITS.IN},
            new EnumComboItem<TAG_UNITS>(){Content = "MM", Value = TAG_UNITS.MM},
            new EnumComboItem<TAG_UNITS>(){Content = "Hidden", Value = TAG_UNITS.HIDDEN},
        };


        private readonly List<EnumComboItem<AREA_TYPE>> _AreaTypeList = new List<EnumComboItem<AREA_TYPE>>()
        {
            new EnumComboItem<AREA_TYPE>(){Content = "DB", Value = AREA_TYPE.DB},
            new EnumComboItem<AREA_TYPE>(){Content = "I", Value = AREA_TYPE.IN},
            new EnumComboItem<AREA_TYPE>(){Content = "Q", Value = AREA_TYPE.OUT},
            new EnumComboItem<AREA_TYPE>(){Content = "M", Value = AREA_TYPE.MEMORY}
        };


        private readonly List<EnumComboItem<PLC_TYPE>> _PLCTypeList = new List<EnumComboItem<PLC_TYPE>>()
        {
            new EnumComboItem<PLC_TYPE>(){Content = "S7-300", Value = PLC_TYPE.S7300},
            new EnumComboItem<PLC_TYPE>(){Content = "S7-1200", Value = PLC_TYPE.S71200},
        };

        private readonly List<EnumComboItem<REQUEST_INTERVAL>> _RequestIntervalList = new List<EnumComboItem<REQUEST_INTERVAL>>()
        {
            new EnumComboItem<REQUEST_INTERVAL>(){Content = "5 Seconds",  Value = REQUEST_INTERVAL.REQUEST_5SEC},
            new EnumComboItem<REQUEST_INTERVAL>(){Content = "2 Seconds", Value = REQUEST_INTERVAL.REQUEST_2SEC},
            new EnumComboItem<REQUEST_INTERVAL>(){Content = "1 Second", Value = REQUEST_INTERVAL.REQUEST_1SEC},
            new EnumComboItem<REQUEST_INTERVAL>(){Content = "500 Milliseconds", Value = REQUEST_INTERVAL.REQUEST_500MSEC},
            new EnumComboItem<REQUEST_INTERVAL>(){Content = "200 Milliseconds", Value = REQUEST_INTERVAL.REQUEST_200MSEC},
        };

        private readonly List<EnumComboItem<READOUT_RESOLUTION>> _ReadoutResolutionListMM = new List<EnumComboItem<READOUT_RESOLUTION>>()
        {
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".02mm",  Value = READOUT_RESOLUTION.READOUT_001},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".01mm", Value = READOUT_RESOLUTION.READOUT_0005},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".002mm", Value = READOUT_RESOLUTION.READOUT_0001},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".001mm", Value = READOUT_RESOLUTION.READOUT_00005}                
        };
        private readonly List<EnumComboItem<READOUT_RESOLUTION>> _ReadoutResolutionListIN = new List<EnumComboItem<READOUT_RESOLUTION>>()
        {
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".001\"",  Value = READOUT_RESOLUTION.READOUT_001},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".0005\"", Value = READOUT_RESOLUTION.READOUT_0005},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".0001\"", Value = READOUT_RESOLUTION.READOUT_0001},
            new EnumComboItem<READOUT_RESOLUTION>(){Content = ".00005\"", Value = READOUT_RESOLUTION.READOUT_00005}                
        }; 
    }
}
