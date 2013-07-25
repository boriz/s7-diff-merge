using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using DMCBase;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows.Data;
using Ionic.Zip;

namespace S7_DMCToolbox
{
    class S7_ViewModel : NotifyPropertyChangedBase
    {
        #region Model instance

        //Model instances
        private S7 _S7Model;
        private S7 S7Model
        {
            get
            {
                if (_S7Model == null)
                {
                    _S7Model = new S7();
                    _S7Model.PropertyChanged += new PropertyChangedEventHandler(_S7Model_PropertyChanged);
                }
                return _S7Model;
            }
        }
        void _S7Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
#endregion

        #region Private Variables
        private ObservableCollection<LogEvent> _LogModel;
        internal CollectionViewSource cvsBlocks { get; set; }
        internal Dictionary<String, Block> dicBlocks
        {
            get
            {
                return S7Model.AllBlocks;
            }
        }
        
      
        private String _ProjectExtendedName = "";
        #endregion

        #region Public Properties for Binding to View
        //Exposed properties for binding
        public ObservableCollection<LogEvent> LogModel
        {
            get
            {
                if (_LogModel == null)
                {
                    _LogModel = new ObservableCollection<LogEvent>();

                    if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    {
                        _LogModel.Add(
                            new LogEvent()
                            {
                                Timestamp = DateTime.Now,
                                Message = "This is an info message",
                                LogSeverity = LogEvent.Severity.Info,
                            });

                        _LogModel.Add(
                            new LogEvent()
                            {
                                Timestamp = DateTime.Now,
                                Message = "This is an warning message",
                                LogSeverity = LogEvent.Severity.Warning,
                            });

                        _LogModel.Add(
                            new LogEvent()
                            {
                                Timestamp = DateTime.Now,
                                Message = "This is an error message",
                                LogSeverity = LogEvent.Severity.Error,
                            });

                        //_LogEvents.Add(
                        //    new LogEvent()
                        //    {
                        //        Timestamp = DateTime.Now,
                        //        Message = "This is a really, really, really, really, really long message",
                        //        LogSeverity = LogEvent.Severity.Error,
                        //    });
                    }
                }
                return _LogModel;
            }
            set
            {
                if (_LogModel != value)
                {
                    _LogModel = value;
                    NotifyPropertyChanged("LogModel");
                }
            }
        }
        public String ProjectPath
        {
            get
            {
                return S7Model.ProjectPath;
            }
            set
            {
                S7Model.ProjectPath = value;
                NotifyPropertyChanged("ProjectPath");
            }
        }
     
        public ICollectionView AllBlocks
        {
            get
            {
                if (cvsBlocks == null)
                    return CollectionViewSource.GetDefaultView(dicBlocks);
                cvsBlocks.Source = dicBlocks;
                return cvsBlocks.View;
            }
        }
        public Boolean IsBusy
        {
            get
            {
                return S7Model.IsBusy;
            }
        }
        public Int16 ProgressBarCurrent
        {
            get
            {
                return S7Model.ProgressBarCurrent;
            }
            set
            {
                S7Model.ProgressBarCurrent = value;
                NotifyPropertyChanged("ProgressBarCurrent");
            }
        }
        public Int16 ProgressBarMax
        {
            get
            {
                return S7Model.ProgressBarMax;
            }
        }
        public String ProjectName
        {
            get
            {
                return  _ProjectExtendedName;
            }
        }
    
        public Boolean udtFilter
        {
            get
            {
                return Properties.Settings.Default.udtFilter;
            }
            set
            {
                Properties.Settings.Default.udtFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("udtFilter");
            }
        }
        public Boolean fbFilter
        {
            get
            {
                return Properties.Settings.Default.fbFilter;
            }
            set
            {
                Properties.Settings.Default.fbFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("fbFilter");
            }
        }
        public Boolean fcFilter
        {
            get
            {
                return Properties.Settings.Default.fcFilter;
            }
            set
            {
                Properties.Settings.Default.fcFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("fcFilter");
            }
        }
        public Boolean vatFilter
        {
            get
            {
                return Properties.Settings.Default.vatFilter;
            }
            set
            {
                Properties.Settings.Default.vatFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("vatFilter");
            }
        }
        public Boolean obFilter
        {
            get
            {
                return Properties.Settings.Default.obFilter;
            }
            set
            {
                Properties.Settings.Default.obFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("obFilter");
            }
        }
        public Boolean dbFilter
        {
            get
            {
                return Properties.Settings.Default.dbFilter;
            }
            set
            {
                Properties.Settings.Default.dbFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("dbFilter");
            }
        }
        public Boolean recentFilter
        {
            get
            {
                return Properties.Settings.Default.recentFilter;
            }
            set
            {
                Properties.Settings.Default.recentFilter = value;
                Properties.Settings.Default.Save();
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("recentFilter");
            }
        }
        public String SelectedOPCServer
        {
            get
            {
                if (S7Model.SelectedOPCServer == null)
                {
                    if (Properties.Settings.Default.RecentOPCServers.Count > 0)
                    {
                        S7Model.SelectedOPCServer = Properties.Settings.Default.RecentOPCServers[0];
                    }
                }
                return S7Model.SelectedOPCServer;
            }
            set
            {
                S7Model.SelectedOPCServer = value;
            }
        }
        public String SelectedAlarmFolder
        {
            get
            {
                if (S7Model.SelectedAlarmFolder == null)
                {
                    if (Properties.Settings.Default.RecentAlarmFolderNames.Count > 0)
                    {
                        S7Model.SelectedOPCServer = Properties.Settings.Default.RecentAlarmFolderNames[0];
                    }
                }
                return S7Model.SelectedAlarmFolder;
            }
            set
            {
                S7Model.SelectedAlarmFolder = value;
            }
        }


        public object SelectedBlock
        {
            get
            {
                return S7Model.CurrentBlock;
            }
            set
            {
                if (value is KeyValuePair<String, Block>)
                {
                    S7Model.CurrentBlock = (KeyValuePair<String, Block>)value;
                }
            }
        }

        #endregion

        #region Commands
        //User actions
        public ICommand BrowseCmd
        {
            get
            {
                return new RelayCommand(p => BrowseWindow(p as String), z => !S7Model.IsBusy);
            }
        }
        public ICommand StartCmd
        {
            get
            {
                return new RelayCommand(p => GetBlocks(), z => !S7Model.IsBusy);
            }
        }
        public ICommand GetAlarmWorxInfoCmd
        {
            get
            {
                return new RelayCommand(p => GetAlarmWorxInfo(), z => !S7Model.IsBusy);
            }
        }
        public ICommand ExportAlarmWorxCmd
        {
            get
            {
                return new RelayCommand(p => ExportAlarmWorx(), z => !S7Model.IsBusy);
            }
        }
        public ICommand ExportKepwareCmd
        {
            get
            {
                return new RelayCommand(p => ExportKepware(), z => !S7Model.IsBusy);
            }
        }

        public ICommand ExportWinCCFlexDigitalAlarmsCmd
        {
            get
            {
                return new RelayCommand(p => ExportWinCCFlexDigitalAlarms(), z => !S7Model.IsBusy);
            }
        }



        private void ExportWinCCFlexDigitalAlarms()
        {
            MessageBox.Show("Not implemented yet");

            VistaSaveFileDialog selectFileDialog = new VistaSaveFileDialog();

            selectFileDialog.Title = "Select Export Location";
            selectFileDialog.AddExtension = true;
            selectFileDialog.DefaultExt = ".csv";
            selectFileDialog.Filter = "CSV File|*.csv";
            if (Directory.Exists(S7Model.WinCCFlexDigitalAlarmsExportFilePath))
                selectFileDialog.InitialDirectory = Properties.Settings.Default.WinCCFlexDigitalAlarmsExportFilePath;

            if ((bool)selectFileDialog.ShowDialog())
            {
                S7Model.WinCCFlexDigitalAlarmsExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.WinCCFlexDigitalAlarmsExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.Save();
                S7Model.ExportWinCCFlexDigitalAlarms();
            }

        }

        private void ExportKepware()
        {
            VistaSaveFileDialog selectFileDialog = new VistaSaveFileDialog();

            selectFileDialog.Title = "Select Export Location";
            selectFileDialog.AddExtension = true;
            selectFileDialog.DefaultExt = ".csv";
            selectFileDialog.Filter = "CSV File|*.csv";
            if (Directory.Exists(S7Model.KepwareExportFilePath))
                selectFileDialog.InitialDirectory = Properties.Settings.Default.KepwareExportFilePath;

            if ((bool)selectFileDialog.ShowDialog())// == DialogResult.OK)
            {
                S7Model.KepwareExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.KepwareExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.Save();
                S7Model.ExportKepware();
            }
            
        }

        private void GetAlarmWorxInfo()
        {
            AlarmWorxEntry modalWindow = new AlarmWorxEntry();
            modalWindow.DataContext = this;
            modalWindow.ShowDialog();
        }

        private void ExportAlarmWorx()
        {
            VistaSaveFileDialog selectFileDialog = new VistaSaveFileDialog();
            selectFileDialog.AddExtension = true;
            selectFileDialog.DefaultExt = ".csv";
            selectFileDialog.Title = "Select Export Location";
            selectFileDialog.Filter = "CSV File|*.csv";
            if (Directory.Exists(S7Model.AlarmWorxExportFilePath))
                selectFileDialog.InitialDirectory = Properties.Settings.Default.AlarmWorxExportFilePath;

            if ((bool)selectFileDialog.ShowDialog())// == DialogResult.OK)
            {
                S7Model.AlarmWorxExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.AlarmWorxExportFilePath = selectFileDialog.FileName;
                Properties.Settings.Default.Save();
                S7Model.ExportAlarmWorx();
            }
            
        }

        public ICommand CancelCmd
        {
            get
            {
                return new RelayCommand(p => Application.Current.MainWindow.Close());
            }
        }

        #endregion

        #region Command Implementations
   
        private void GetBlocks()
        {
            S7Model.GetBlocks();
        }

        //UI Action implementations
        private void BrowseWindow(String Source)
        {
            string strSelectedFolder = string.Empty;
            VistaOpenFileDialog selectFileDialog = new VistaOpenFileDialog();

            selectFileDialog.Title = "Select S7 project.";
            
            if (Directory.Exists(S7Model.ProjectPath))
                selectFileDialog.InitialDirectory = Properties.Settings.Default.ProjectPath;
            
            selectFileDialog.Filter = "S7 Projects(*.zip, *.s7p, *.s7l)|*.s7p;*.s7l;*.zip";
            if ((bool)selectFileDialog.ShowDialog())// == DialogResult.OK)
            {
                ProjectPath = selectFileDialog.FileName;
                Properties.Settings.Default.ProjectPath = ProjectPath;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        #region Program Initiazation
        //Command line interface
        internal void InitFromCommandLineArguments(string[] Args)
        {
            //Args is made up of:
            //0: %base file path
            //1: %bname
            //2: %mine
            //3: %yname
            if (Properties.Settings.Default.RecentlyUsedBlocks == null)
            {
                Properties.Settings.Default.RecentlyUsedBlocks = new System.Collections.Specialized.StringCollection();
            }
            if (Properties.Settings.Default.RecentOPCServers == null)
            {
                Properties.Settings.Default.RecentOPCServers = new System.Collections.Specialized.StringCollection();
            }
            if (Properties.Settings.Default.RecentAlarmFolderNames == null)
            {
                Properties.Settings.Default.RecentAlarmFolderNames = new System.Collections.Specialized.StringCollection();
            }
            cvsBlocks = new CollectionViewSource();
            cvsBlocks.Filter += new FilterEventHandler(cvsBlocks_Filter);
            if ((Args != null) && (Args.Count() > 0))
            {
                ProjectPath = Args[0];
            }
        }
        #endregion

        #region Event Handlers
        void cvsBlocks_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
          
            if ((!dbFilter) && (!fbFilter) && (!fcFilter) && (!udtFilter) && (!obFilter) && (!vatFilter))
            {
                e.Accepted = true;
            }
            
            if (e.Item is KeyValuePair<String, Block>)
            {
                KeyValuePair<String, Block> KVP = (KeyValuePair<String, Block>)e.Item;
                if ((dbFilter) && (KVP.Value.Name.ToLower().StartsWith("db")))
                {
                    e.Accepted = true;
                }
                if ((udtFilter) && (KVP.Value.Name.ToLower().StartsWith("udt")))
                {
                    e.Accepted = true;
                }
                if ((fbFilter) && (KVP.Value.Name.ToLower().StartsWith("fb")))
                {
                    e.Accepted = true;
                }
                if ((fcFilter) && (KVP.Value.Name.ToLower().StartsWith("fc")))
                {
                    e.Accepted = true;
                }
                if ((obFilter) && (KVP.Value.Name.ToLower().StartsWith("ob")))
                {
                    e.Accepted = true;
                }
                if ((vatFilter) && (KVP.Value.Name.ToLower().StartsWith("vat")))
                {
                    e.Accepted = true;
                }
                if ((recentFilter) && (!Properties.Settings.Default.RecentlyUsedBlocks.Contains(KVP.Key)))
                {
                    e.Accepted = false;
                }
             
            }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            S7Model.ClearTempDirectories();
        }
        #endregion
    }
}

