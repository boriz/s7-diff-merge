using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Trending
{
    public class ProfinetViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        public ProfinetViewModel()
        {
          
           
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

        public Model Model
        {
            get { return Model.Instance; }
        }

        public ProfinetModel ConnectedDisplay
        {
            get { return Model.ConnectedDevice as ProfinetModel; }
        }

      
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
