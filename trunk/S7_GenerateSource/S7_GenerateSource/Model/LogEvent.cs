using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMCBase;

namespace S7_GenerateSource
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


    public static class EventFire
    {
        public static event EventHandler<EventArgs<LogEvent>> LogEvent;
        public static void Trace(string message, params object[] args)
        {
            OnLogEventOccurred(string.Format(message, args), S7_GenerateSource.LogEvent.Severity.Trace);
        }

        public static void Info(string message, params object[] args)
        {
            OnLogEventOccurred(string.Format(message, args), S7_GenerateSource.LogEvent.Severity.Info);
        }

        public static void Warning(string message, params object[] args)
        {
            OnLogEventOccurred(string.Format(message, args), S7_GenerateSource.LogEvent.Severity.Warning);
        }

        public static void Error(string message, object[] args)
        {
            OnLogEventOccurred(string.Format(message, args), S7_GenerateSource.LogEvent.Severity.Error);
        }

        public static void Error(string message)
        {
            OnLogEventOccurred(message, S7_GenerateSource.LogEvent.Severity.Error);
        }

        public static void OnLogEventOccurred(string message, LogEvent.Severity severity)
        {
            EventHandler<EventArgs<LogEvent>> handler = LogEvent;

            if (handler != null)
            {
                handler(
                    null,
                    new EventArgs<LogEvent>(
                        new LogEvent()
                        {
                            LogSeverity = severity,
                            Message = message,
                            Timestamp = DateTime.Now,
                        }));
            }
        }
    }

}
