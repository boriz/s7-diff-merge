using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using Windows_Display_App;

namespace Trending
{
    public class TrendTagParameters : Parameters
    {
        public const string UnitsName = "p004";

        public event EventHandler<ParameterEventArgs> ParameterChangedEvent;
        public event EventHandler<ParameterEventArgs> LocalParameterChangedEvent;

        #region Constructor
        public TrendTagParameters(int tag)
        {
            Tag = tag;
//            TypeParameter.PropertyChanged += TypeParameter_PropertyChanged;
//            DisplayUnitsParameter.PropertyChanged += DisplayUnitsParameter_PropertyChanged;
        }

        private void DisplayUnitsParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("ReadoutResolutionSource");
        }

        //Future use. PLC type.
        void TypeParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
    //        if (TypeParameter.Value == GAGE_TYPE.DIGIMATIC)
      //      {
        //        HoldEnabledParameter.Value = false;
          //      if (ModeParameter.Value == MEASUREMENT_MODE.TIR)
            //        ModeParameter.Value = MEASUREMENT_MODE.INCR;
 //           }
   //         NotifyPropertyChanged("DigiEnabled");
     //       NotifyPropertyChanged("QuadEnabled");
       //     NotifyPropertyChanged("ModeSource");
        }
        #endregion  

        #region Properties




        #endregion

        #region MicroParameters
        public Parameter<bool> TagEnabledParm
        {
            get { return _tagEnabledParm; }
        }
        private readonly Parameter<bool> _tagEnabledParm = new Parameter<bool>
        {
            Name = "p003",
            Display = "Channel Enabled",
            Min = 0.0,
            Max = 1.0,
            Type = typeof(bool),
            Note = "Is Channel Enabled"
        };


        public Parameter<TAG_UNITS> DisplayUnitsParameter
        {
            get { return _displayUnitsParameter; }
        }
        private readonly Parameter<TAG_UNITS> _displayUnitsParameter = new Parameter<TAG_UNITS>
        {
            Name = UnitsName,
            Display = "Display Units",
            Min = 0.0,
            Max = 2.0,
            Type = typeof(TAG_UNITS),
            Note = "Units to be displayed for this gage"
        };


        public Parameter<AREA_TYPE> AreaTypeParameter
        {
            get { return _areaTypeParameter; }
        }
        private readonly Parameter<AREA_TYPE> _areaTypeParameter = new Parameter<AREA_TYPE>
        {
            Name = "p017",
            Display = "Units",
            Min = 0.0,
            Max = 6.0,
            Type = typeof(AREA_TYPE),
            Note = "Area types to be displayed"
        };


        public Parameter<int> DbNumberParameter
        {
            get { return _dbNumberParameter; }
        }

        private readonly Parameter<int> _dbNumberParameter = new Parameter<int>
        {
            Name = "DBNumber",
            Display = "DB Number",
            Min = 1,
            Max = 100,
            Type = typeof(int),
            Note = "DB Number"
        };




        public Parameter<int> BaseAddressParameter
        {
            get { return _baseAddressParameter; }
        }

        private readonly Parameter<int> _baseAddressParameter = new Parameter<int>
        {
            Name = "Starting address",
            Display = "Byte number",
            Min = 1,
            Max = 10000,
            Type = typeof(int),
            Note = "Byte number"
        };


        public Parameter<DATA_TYPES> DataTypeParameter
        {
            get { return _dataTypeParameter; }
        }

        private readonly Parameter<DATA_TYPES> _dataTypeParameter = new Parameter<DATA_TYPES>
        {
            Name = "Data types",
            Display = "Data type",
            Min = 0,
            Max = 3,
            Type = typeof (int),
            Note = "Data type"
        };


        public Parameter<bool> ToleranceEnabledParameter
        {
            get { return _toleranceEnabledParameter; }
        }
        private readonly Parameter<bool> _toleranceEnabledParameter = new Parameter<bool>
        {
            Name = "p009",
            Display = "Tolerance",
            Type = typeof(bool),
            Note = "Enable/Disable Tolerance Mode"
        };

        public Parameter<double> HighToleranceParameter
        {
            get { return _highToleranceParameter; }
        }
        private readonly Parameter<double> _highToleranceParameter = new Parameter<double>
        {
            Name = "p910",
            Display = "High Tolerance",
            Type = typeof(double),
            Note = "High Tolerance Value"
        };

        public Parameter<double> LowToleranceParameter
        {
            get { return _lowToleranceParameter; }
        }
        private readonly Parameter<double> _lowToleranceParameter = new Parameter<double>
        {
            Name = "p911",
            Display = "Low Tolerance",
            Type = typeof(double),
            Note = "Low Tolerance Value"
        };

        #endregion


        #region App Parameters

        public Parameter<bool> AutomaticRequestParameter
        {
            get { return _automaticRequestParameter; }
        }
        private readonly Parameter<bool> _automaticRequestParameter = new Parameter<bool>
        {
            Name = "pxx2",
            Display = "Automatic Request",
            Type = typeof(bool),
            Note = "Automatically request Digimatic data on set interval"
        };

        public Parameter<bool> AutomaticLogParameter
        {
            get { return _automaticLogParameter; }
        }
        private readonly Parameter<bool> _automaticLogParameter = new Parameter<bool>
        {
            Name = "pxx3",
            Display = "Automatic Log",
            Type = typeof(bool),
            Note = "Automatically log data on set interval"
        };


        public Parameter<float> LogIntervalParameter
        {
            get { return _logIntervalParameter; }
        }
        private readonly Parameter<float> _logIntervalParameter = new Parameter<float>
        {
            Name = "pxx5",
            Display = "Log Interval (s)",
            Min = 1.0,
            Type = typeof(float),
            Note = "Interval of automatic logging"
        };



        public Parameter<bool> ShowAnalogScaleParameter
        {
            get { return _showAnalogScaleParameter; }
        }
        private readonly Parameter<bool> _showAnalogScaleParameter = new Parameter<bool>
        {
            Name = "pxx6",
            Display = "Show Analog Scale",
            Min = 1.0,
            Type = typeof(bool),
            Note = "Show analog scale on readout channel"
        };

        public Parameter<bool> ShowMinMaxParameter
        {
            get { return _showMinMaxParameter; }
        }
        private readonly Parameter<bool> _showMinMaxParameter = new Parameter<bool>
        {
            Name = "pxx7",
            Display = "Show Min/Max Values",
            Min = 1.0,
            Type = typeof(bool),
            Note = "Show Min/Max Values on readout channel"
        };


        #endregion

        #region Methods
        public override void BuildDictionary()
        {
            MicroParameters = new Dictionary<string, BaseParameter>()
            {
                {TagEnabledParm.Name, TagEnabledParm},
                {ToleranceEnabledParameter.Name, ToleranceEnabledParameter},
                {HighToleranceParameter.Name, HighToleranceParameter},
                {LowToleranceParameter.Name, LowToleranceParameter},
            };

            LocalParameters = new List<BaseParameter>()
            {
                AutomaticRequestParameter,
                AutomaticLogParameter,
                ShowAnalogScaleParameter,
                ShowMinMaxParameter,
            };

            foreach (var parm in MicroParameters.Values)
            {
                parm.ValueChangedEvent += microParm_ValueChangedEvent;
            }

            foreach (var parm in LocalParameters)
            {
                parm.ValueChangedEvent += localParm_ValueChangedEvent;
            }

        }

        void localParm_ValueChangedEvent(object sender, EventArgs e)
        {
            EventHandler<ParameterEventArgs> handler = LocalParameterChangedEvent;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs(sender as BaseParameter));
            }
        }

        void microParm_ValueChangedEvent(object sender, EventArgs e)
        {
            EventHandler<ParameterEventArgs> handler = ParameterChangedEvent;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs(sender as BaseParameter));
            }
        }
        #endregion
    }
}
