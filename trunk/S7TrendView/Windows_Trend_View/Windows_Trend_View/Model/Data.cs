using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Windows_Trend_View
{
    public class DataSample
    {
        public DateTime Time { get; set; }
        public string ActualValue { get; set; }
        //public string Units { get; set; }
    }

    public class TagData
    {
        public ObservableCollection<DataSample> Data { get; set; }
        public TagData()
        {
            Data = new ObservableCollection<DataSample>() { };
        }
    }


    public class ActualData
    {
        public string Current;
        public string Min;
        public string Max;
    }
}
