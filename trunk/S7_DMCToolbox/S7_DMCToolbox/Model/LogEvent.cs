using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMCBase;

namespace S7_DMCToolbox
{
    public class LogEvent : NotifyPropertyChangedBase
    {
        public enum Severity
        {
            Trace,
            Info,
            Warning,
            Error,
        }

        private DateTime _Timestamp;
        public DateTime Timestamp
        {
            get
            {
                return _Timestamp;
            }
            set
            {
                _Timestamp = value;
                NotifyPropertyChanged("Timestamp");
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private Severity _LogSeverity;
        public Severity LogSeverity
        {
            get
            {
                return _LogSeverity;
            }
            set
            {
                _LogSeverity = value;
                NotifyPropertyChanged("LogSeverity");
            }
        }
    }
}
