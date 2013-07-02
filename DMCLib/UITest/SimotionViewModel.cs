using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMCBase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace UITest
{
    public class SimotionViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<SimotionProject> _Projects;
        public ObservableCollection<SimotionProject> Projects
        {
            get
            {
                if (_Projects == null)
                {
                    _Projects = new ObservableCollection<SimotionProject>();

                    //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    {
                        _Projects.Add(
                            new SimotionProject()
                            {
                                Name = "Project 1",
                                IsSelected = true,
                            });

                        _Projects.Add(
                            new SimotionProject()
                            {
                                Name = "Project 2",
                                IsSelected = false,
                            });

                        _Projects.Add(
                            new SimotionProject()
                            {
                                Name = "Project 3",
                                IsSelected = true,
                            });

                        _Projects.Add(
                            new SimotionProject()
                            {
                                Name = "Project 4",
                                IsSelected = false,
                            });
                    }
                }
                return _Projects;
            }
            set
            {
                if (_Projects != value)
                {
                    _Projects = value;
                    NotifyPropertyChanged("Projects");
                }
            }
        }

        private ObservableCollection<LogEvent> _LogEvents;
        public ObservableCollection<LogEvent> LogEvents
        {
            get
            {
                if (_LogEvents == null)
                {
                    _LogEvents = new ObservableCollection<LogEvent>();

                    //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
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

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }

        public ICommand SelectAllCmd { get; private set; }
        public ICommand SelectNoneCmd { get; private set; }

        public SimotionViewModel()
        {
            SelectAllCmd = new RelayCommand(p => SelectAll(), z => !IsBusy);
            SelectNoneCmd = new RelayCommand(p => SelectNone(), z => !IsBusy);
        }
        
        private void SelectAll()
        {
            foreach (SimotionProject proj in Projects)
            {
                proj.IsSelected = true;
            }
        }

        private void SelectNone()
        {
            foreach (SimotionProject proj in Projects)
            {
                proj.IsSelected = false;
            }
        }
    }
}
