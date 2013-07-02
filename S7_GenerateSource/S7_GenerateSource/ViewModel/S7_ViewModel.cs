using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using DMCBase;
using System.Collections.ObjectModel;
using SimaticLib;
using System.Windows.Input;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows.Data;
using System.Windows.Threading;
using System.Deployment.Application;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;


namespace S7_GenerateSource
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
        internal CollectionViewSource cvsBlocks { get; set; }
        internal Dictionary<String, Blocks> dicBlocks
        {
            get
            {
                return S7Model.AllBlocks;
            }
        }
        private Boolean _DifferenceFilter;
        private Boolean _OrphanFilter;
        private String _LeftProjectExtendedName = "";
        private String _RightProjectExtendedName = "";
        private String _StatusBarText = "Ready";
        #endregion

        #region Public Properties for Binding to View
        //Exposed properties for binding
        private ObservableCollection<LogEvent> _LogEvents;
        public ObservableCollection<LogEvent> LogEvents
        {
            get
            {
                if (_LogEvents == null)
                {
                    _LogEvents = new ObservableCollection<LogEvent>();

                    if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    {
                        _LogEvents.Add(
                            new LogEvent()
                            {
                                Timestamp = DateTime.Now,
                                Message = "This is an info message",
                                LogSeverity = LogEvent.Severity.Info,
                            });

                        _LogEvents.Add(
                            new LogEvent()
                            {
                                Timestamp = DateTime.Now,
                                Message = "This is an warning message",
                                LogSeverity = LogEvent.Severity.Warning,
                            });

                        _LogEvents.Add(
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
                return _LogEvents;
            }
            set
            {
                if (_LogEvents != value)
                {
                    _LogEvents = value;
                    NotifyPropertyChanged("LogEvents");
                }
            }
        }

        public String LeftProjectPath
        {
            get
            {
                return S7Model.LeftProjectPath;
            }
            set
            {
                S7Model.LeftProjectPath = value;
                NotifyPropertyChanged("LeftProjectPath");
                NotifyPropertyChanged("IsLeftProjectZipped");
            }
        }

        public String RightProjectPath
        {
            get
            {
                return S7Model.RightProjectPath;
            }
            set
            {
                S7Model.RightProjectPath = value;
                NotifyPropertyChanged("RightProjectPath");
                NotifyPropertyChanged("IsRightProjectZipped");
            }
        }

        public String ExtractRightProjectPath
        {
            get
            {
                return this.S7Model.ExtractRightProjectPath;
            }
        }

        public String ExtractLeftProjectPath
        {
            get
            {
                return this.S7Model.ExtractLeftProjectPath;
            }
        }

        public Dictionary<String, Block> LeftBlocks
        {
            get
            {                

                return S7Model.LeftBlocks;
            }
        }
        public Dictionary<String, Block> RightBlocks
        {
            get
            {

                return S7Model.RightBlocks;
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

        public Boolean IsNotBusy
        {
            get
            {
                return !S7Model.IsBusy;
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

        public String LeftProjectName
        {
            get
            {
                return  _LeftProjectExtendedName;
            }
        }

        public String RightProjectName
        {
            get
            {
                return _RightProjectExtendedName;
            }
        }

        public Boolean IsLeftProjectZipped
        {
            get
            {
                return LeftProjectPath.ToUpper().EndsWith(".ZIP");
            }
        }

        public Boolean IsRightProjectZipped
        {
            get
            {
                return RightProjectPath.ToUpper().EndsWith(".ZIP");
            }
        }

        public Boolean OrphanFilter
        {
            get
            {
                return _OrphanFilter;
            }
            set
            {
                _OrphanFilter = value;
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("OrphanFilter");
            }
        }
        public Boolean DifferenceFilter
        {
            get
            {
                return _DifferenceFilter;
            }
            set
            {
                _DifferenceFilter = value;
                if (cvsBlocks.View != null)
                    cvsBlocks.View.Refresh();
                NotifyPropertyChanged("DifferenceFilter");
            }
        }
        public object SelectedBlock { get; set; }

        public String StatusBarText
        {
            get
            {
                return _S7Model.StatusBarText;
            }
        }
        private  Dispatcher dispatcher;
        
        private void LogNewEvent(LogEvent newEvent)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(
                    new Action(
                        delegate
                        {
                            LogNewEvent(newEvent);
                        }
                    ));
            }
            else
            {
                if (newEvent.LogSeverity != LogEvent.Severity.Trace)
                    LogEvents.Add(newEvent);

                ////Add to nLog
                //switch (newEvent.LogSeverity)
                //{
                //    case LogEvent.Severity.Error:
                //        logger.Error(newEvent.Message);
                //        break;
                //    case LogEvent.Severity.Info:
                //        logger.Info(newEvent.Message);
                //        break;
                //    case LogEvent.Severity.Warning:
                //        logger.Warn(newEvent.Message);
                //        break;
                //    case LogEvent.Severity.Trace:
                //        logger.Trace(newEvent.Message);
                //        break;
                //}

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
        
        public ICommand SaveCmd
        {
            get
            {
                return new RelayCommand(p => Save(), z => !S7Model.IsBusy);
            }
        }

        public ICommand CopyBlockToRightCmd
        {
            get
            {
                return new RelayCommand(p => CopyBlockToRight(), z => !S7Model.IsBusy);
            }
        }

        public ICommand MergeBlockToRightCmd
        {
            get
            {
                return new RelayCommand(p => StartDiffProcess(), z => !S7Model.IsBusy);
            }
        }

        public ICommand CancelCmd
        {
            get
            {
                return new RelayCommand(p => Application.Current.MainWindow.Close());
            }
        }

        public ICommand UpdateApplicationCmd
        {
            get
            {
                return new RelayCommand(p => InstallUpdateSyncWithInfo());
            }
        }

        #endregion

        #region Command Implementations

        private void StartDiffProcess()
        {
            try
            {
                if ((SelectedBlock != null) && (SelectedBlock is KeyValuePair<String, Blocks>))
                {
                    KeyValuePair<String, Blocks> KVP = (KeyValuePair<String, Blocks>)SelectedBlock;
                    S7Model.CurrentBlock = KVP;
                    S7Model.StartDiffProcess();

                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        private void CopyBlockToRight()
        {
            try
            {
                if ((SelectedBlock != null) && (SelectedBlock is KeyValuePair<String, Blocks>))
                {
                    KeyValuePair<String, Blocks> KVP = (KeyValuePair<String, Blocks>)SelectedBlock;
                    if ((KVP.Value.LeftBlock.Type != PLCBlockType.SourceBlock && KVP.Value.LeftBlock.Language == PLCLanguage.SCL) ||
                        (KVP.Value.RightBlock.Type != PLCBlockType.SourceBlock && KVP.Value.RightBlock.Language == PLCLanguage.SCL))
                    {
                        MessageBox.Show("You are trying to copy block compiled from the SCL source. You have to copy SCL source block manually.", "Warning!", MessageBoxButton.OK);
                    }

                    if (MessageBox.Show("Are you sure you want to copy block <" + KVP.Key + "> to the right project?", "Warning. Overwriting block.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        S7Model.CurrentBlock = KVP;
                        S7Model.CopyBlockToRight();
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        private void Save()
        {
            try
            {
                if (S7Model.RightProjectPath.ToLower().EndsWith(".zip"))
                {
                    if (MessageBox.Show("Are you sure you want to save right project?", "Warning. Overwriting archive.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        S7Model.Save();
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        private void GetBlocks()
        {
            try
            {
                if (ExtractLeftProjectPath != "" || ExtractRightProjectPath != "")
                {
                    if (MessageBox.Show("Are you sure you want to reload projects?", "Warning. Unsave changes.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        // Reload both projects
                        S7Model.GetBlocks();
                    }
                }
                else
                {
                    // We don't have unpacked projects - just reload
                    S7Model.GetBlocks();
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        //UI Action implementations
        private void BrowseWindow(String Source)
        {
            try
            {
                string strSelectedFolder = string.Empty;
                VistaOpenFileDialog selectFileDialog = new VistaOpenFileDialog();

                selectFileDialog.Title = "Select S7 project.";
                if (Source.Equals("Left"))
                {
                    if (Directory.Exists(S7Model.LeftProjectPath))
                        selectFileDialog.InitialDirectory = Properties.Settings.Default.LeftProjectPath;
                }
                else if (Source.Equals("Right"))
                {
                    if (Directory.Exists(S7Model.RightProjectPath))
                        selectFileDialog.InitialDirectory = Properties.Settings.Default.RightProjectPath;
                }

                selectFileDialog.Filter = "S7 Projects(*.zip, *.s7p, *.s7l)|*.s7p;*.s7l;*.zip";
                if ((bool)selectFileDialog.ShowDialog())// == DialogResult.OK)
                {
                    if (Source.Equals("Left"))
                    {
                        LeftProjectPath = selectFileDialog.FileName;
                        Properties.Settings.Default.LeftProjectPath = LeftProjectPath;
                    }
                    else if (Source.Equals("Right"))
                    {
                        RightProjectPath = selectFileDialog.FileName;
                        Properties.Settings.Default.RightProjectPath = RightProjectPath;
                    }
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
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
            
            cvsBlocks = new CollectionViewSource();
            cvsBlocks.Filter += new FilterEventHandler(cvsBlocks_Filter);
            if ((Args != null) && (Args.Count() > 3))
            {
                LeftProjectPath = Args[0];
                RightProjectPath = Args[2];

                if (Args[1].Contains(":"))
                    _LeftProjectExtendedName = "(" + Args[1].Remove(Args[1].IndexOf(":")).Trim() + ")";
                if (Args[3].Contains(":"))
                    _RightProjectExtendedName = "(" + Args[3].Remove(Args[3].IndexOf(":")).Trim() + ")";
            }
            EventFire.LogEvent += new EventHandler<EventArgs<LogEvent>>(S7Model_LogEvent);
            dispatcher = Dispatcher.CurrentDispatcher;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateCompleted +=new CheckForUpdateCompletedEventHandler(ad_CheckForUpdateCompleted);
                try
                {
                    ad.CheckForUpdateAsync();
                }
                catch (Exception e)
                {
                    EventFire.Error(e.ToString());
                }
            }
        }

        void  ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            InstallUpdateSyncWithInfo();
        }

        void S7Model_LogEvent(object sender, EventArgs<LogEvent> e)
        {
            LogNewEvent(e.Value);
        }
        #endregion

        #region Update application version
        private void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                
                try
                {
                    info = ad.CheckForDetailedUpdate();
                    
                }
                catch (DeploymentDownloadException dde)
                {
               //     MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
               //     MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
              //      MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        MessageBoxResult dr = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButton.OKCancel);
                        if (!(MessageBoxResult.OK == dr))
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        MessageBox.Show("This application has detected a mandatory update from your current " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will now install the update and restart.",
                            "Update Available", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            MessageBox.Show("The application has been upgraded, and will now shut down. Please restart.");
                            Application.Current.Shutdown();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            return;
                        }
                    }
                }
               
            }
        }
        #endregion


        #region Event Handlers
        void cvsBlocks_Filter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = false;
                if (e.Item is KeyValuePair<String, Blocks>)
                {
                    KeyValuePair<String, Blocks> KVP = (KeyValuePair<String, Blocks>)e.Item;
                    if ((OrphanFilter) && (KVP.Value.LeftBlock.ModifiedSimilarity == BlockSimilarityType.Orphan))
                    {
                        e.Accepted = true;
                    }
                    if ((DifferenceFilter) && (
                        (KVP.Value.LeftBlock.NameSimilarity == BlockSimilarityType.Different) ||
                        (KVP.Value.LeftBlock.ModifiedSimilarity == BlockSimilarityType.Different) ||
                        (KVP.Value.LeftBlock.SizeSimilarity == BlockSimilarityType.Different) ||
                        (KVP.Value.LeftBlock.SymbolicNameSimilarity == BlockSimilarityType.Different))
                        )
                    {
                        e.Accepted = true;
                    }
                    if ((!OrphanFilter) && (!DifferenceFilter))
                    {
                        e.Accepted = true;
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                S7Model.ClearTempDirectories();
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }
        #endregion
    }
}

