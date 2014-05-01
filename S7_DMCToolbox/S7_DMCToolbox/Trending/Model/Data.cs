using S7_DMCToolbox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Trending
{
    public class DataSample
    {
        public DateTime Time { get; set; }
        public string ActualValue { get; set; }
        //public string Units { get; set; }
    }

    public class TagData
    {
        public Tag TagInfo { get; set; }
        public ObservableCollection<DataSample> Data { get; set; }
        public TagData()
        {
            TagInfo = new Tag();
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
