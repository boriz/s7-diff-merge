using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Windows_Trend_View
{
    public class ProfinetViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        public ProfinetViewModel()
        {
            Tag1Parms.TagEnabledParm.PropertyChanged += TagEnabledParm_PropertyChanged;
            Tag2Parms.TagEnabledParm.PropertyChanged += TagEnabledParm_PropertyChanged;
            Tag3Parms.TagEnabledParm.PropertyChanged += TagEnabledParm_PropertyChanged;
            Tag4Parms.TagEnabledParm.PropertyChanged += TagEnabledParm_PropertyChanged;

            Tag1Parms.AutomaticLogParameter.PropertyChanged += AutomaticLogParameterOnPropertyChanged;
            Tag2Parms.AutomaticLogParameter.PropertyChanged += AutomaticLogParameterOnPropertyChanged;
            Tag3Parms.AutomaticLogParameter.PropertyChanged += AutomaticLogParameterOnPropertyChanged;
            Tag4Parms.AutomaticLogParameter.PropertyChanged += AutomaticLogParameterOnPropertyChanged;

              //Future
 //           Tag1Parms.LogIntervalParameter.PropertyChanged += LogIntervalParm_PropertyChanged;
 //           Tag2Parms.LogIntervalParameter.PropertyChanged += LogIntervalParm_PropertyChanged;
   //         Tag3Parms.LogIntervalParameter.PropertyChanged += LogIntervalParm_PropertyChanged;
     //       Tag4Parms.LogIntervalParameter.PropertyChanged += LogIntervalParm_PropertyChanged;
        }

        private void AutomaticLogParameterOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NotifyPropertyChanged("LogStatus");
            NotifyPropertyChanged("LogStatusText");
            NotifyPropertyChanged("LogStatusBrush");
        }
        private void TagEnabledParm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("DisplayRows");
            NotifyPropertyChanged("LogStatus");
            NotifyPropertyChanged("LogStatusText");
            NotifyPropertyChanged("LogStatusBrush");
        }
        void LogIntervalParm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("LogInterval");
        }
        #endregion

        #region Properties
        public Model Model
        {
            get { return Model.Instance; }
        }

        public ProfinetModel ConnectedDisplay
        {
            get { return Model.ConnectedDevice as ProfinetModel; }
        }

        # region Parameter Dictionary Properties
        public TrendTagParameters Tag1Parms
        {
            get { return ConnectedDisplay.TagParameters[0]; }
        }

        public TrendTagParameters Tag2Parms
        {
            get { return ConnectedDisplay.TagParameters[1]; }
        }

        public TrendTagParameters Tag3Parms
        {
            get { return ConnectedDisplay.TagParameters[2]; }
        }

        public TrendTagParameters Tag4Parms
        {
            get { return ConnectedDisplay.TagParameters[3]; }
        }

        //For trending
        public TagData Tag1Data
        {
            get { return ConnectedDisplay.TagSamples[0]; }
        }

        public TagData Tag2Data
        {
            get { return ConnectedDisplay.TagSamples[1]; }
        }

        public TagData Tag3Data
        {
            get { return ConnectedDisplay.TagSamples[2]; }
        }

        public TagData Tag4Data
        {
            get { return ConnectedDisplay.TagSamples[3]; }
        }


        public bool LogStatus
        {
            get
            {
                return Tag1Parms.AutomaticLogParameter.Value && Tag1Parms.TagEnabledParm.Value ||
                       Tag2Parms.AutomaticLogParameter.Value && Tag2Parms.TagEnabledParm.Value ||
                       Tag3Parms.AutomaticLogParameter.Value && Tag3Parms.TagEnabledParm.Value ||
                       Tag4Parms.AutomaticLogParameter.Value && Tag4Parms.TagEnabledParm.Value;
            }
            set
            {
                foreach (var channel in ConnectedDisplay.TagParameters.Where(c => c.TagEnabledParm.Value))
                {
                    channel.AutomaticLogParameter.Value = value;
                }
            }
        }
        public string LogStatusText
        {
            get
            {
                return LogStatus ? "Running" : "Inactive";
            }
        }
        public Brush LogStatusBrush
        {
            get
            {
                return LogStatus ? new SolidColorBrush(Colors.LimeGreen)
                        : new SolidColorBrush(Colors.LightGray);
            }
        }
//        public float LogInterval
  //      {
 //           get
  //          {
   //             if (new[]
    //            {
     //               Tag1Parms.LogIntervalParameter.Value,
      //              Tag2Parms.LogIntervalParameter.Value,
       //             Tag3Parms.LogIntervalParameter.Value,
        //            Channel4Parms.LogIntervalParameter.Value
         //       }.Distinct().Count() == 1)
          //      {
           //         return Channel1Parms.LogIntervalParameter.Value;
            //    }
             //   return float.NaN;
           // }
            //set
           // {
            //    Channel1Parms.LogIntervalParameter.Value = value;
             //   Channel2Parms.LogIntervalParameter.Value = value;
              //  Channel3Parms.LogIntervalParameter.Value = value;
               // Channel4Parms.LogIntervalParameter.Value = value;
            
    //    }

        public int DisplayRows
        {
            get { return ConnectedDisplay.TagParameters.Count(p => p.TagEnabledParm.Value) > 1 ? 2:1; }
        }

        #endregion
        #endregion

        private ICommand _requestCommand;
    //    public ICommand RequestCommand
    //    {
    //        get { return _requestCommand ?? (_requestCommand = new RelayCommand(param => Model.SendRequestAll())); }
    //    }

        private ICommand _logCommand;
    //    public ICommand LogCommand
     //   {
      //      get { return _logCommand ?? (_logCommand = new RelayCommand(param => Model.RequestLogAll())); }
       // }

        private ICommand _clearCommand;
   //     public ICommand ClearCommand
   //     {
    //        get { return _clearCommand ?? (_clearCommand = new RelayCommand(param => Model.SendClearAll())); }
     //   }
    }
}
