//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Windows;
//using DMC.CMonster;
//using DMCBase;
//using NLog;
//using NLog.Config;
//using NLog.Targets;
//using System.ComponentModel;

//namespace DMC.UI
//{
//    public partial class DMCWindow : Window, INotifyPropertyChanged
//    {
//        private bool _IsBusy = false;
//        public bool IsBusy
//        {
//            get
//            {
//                return _IsBusy;
//            }
//            set
//            {
//                if (_IsBusy != value)
//                {
//                    _IsBusy = value;
//                    NotifyPropertyChanged("IsBusy");
//                }
//            }
//        }

//        protected CMonster<DMCWindow> CMonster { get; set; }

//        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        
        
//        public DMCWindow()
//        {
//            DebugLogManager.Initialize();

//            this.Closing += new System.ComponentModel.CancelEventHandler(DMCWindow_Closing);

//            this.CMonster = new CMonster<DMCWindow>(this)
//            {
//                CData = GetSimulatedCData(),
//            };
//        }

//        private void DMCWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            this.CMonster.FireEvent("Exit Requested");

//            // TODO:  This should be handled cleaner in the future.  For now, simply give the exit actions a set amount of time to complete before shutting the state machine down
//            Thread.Sleep(50);

//            this.CMonster.Shutdown();
//        }

//        //TODO: load from logic file instead of this
//        private CData GetSimulatedCData()
//        {
//            return new CData()
//            {
//                new CStateData()
//                {
//                    State = "Idle",
//                    EventActions = new List<CEventData>()
//                    {
//                        new CEventData()
//                        {
//                            Event = "About Window Requested",
//                            Actions = new List<string>()
//                            {
//                                "ShowAboutWindow",
//                            },
//                        },
//                        new CEventData()
//                        {
//                            Event = "Exit Requested",
//                            Actions = new List<string>()
//                            {
//                                "CloseChildWindows",
//                            },
//                        },
//                    },
//                },
//            };
//        }

//        #region INotifyPropertyChanged

//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void NotifyPropertyChanged(string propertyIn)
//        {
//            PropertyChangedEventHandler handler = this.PropertyChanged;

//            if (handler != null)
//            {
//                handler(this, new PropertyChangedEventArgs(propertyIn));
//            }
//        }

//        #endregion
//    }
//}
