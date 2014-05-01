using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;

namespace Windows_Trend_View
{
    public class ParameterEventArgs : EventArgs
    {
        public BaseParameter Parameter { get; internal set; }
        public ParameterEventArgs(BaseParameter parm)
        {
            this.Parameter = parm;
        }
    }
    public class Parameters : EnumSources
    {
        //Tag number
        public int Tag { get; set; }

        public Parameters()
        {
            Tag = -1;         

            // Create parameter dictionary. Abstract so that different devices can 
            // specify different parameter lists
            BuildDictionary();
        }

        public virtual void BuildDictionary()
        {}

        public void FireLogEvent()
        {
            _logFlag = true;
            NotifyPropertyChanged("LogFlag");
        }
        private bool _logFlag;
        public bool LogFlag
        {
            get
            {
                if (!_logFlag) return false;
                _logFlag = false;
                NotifyPropertyChanged("LogFlag");
                return true;
            }
        }

        public Dictionary<string, BaseParameter> MicroParameters { get; set; }
        public List<BaseParameter> LocalParameters { get; set; }
    }

    /// <summary>
    /// Custom class for device parameters
    /// </summary>
    public abstract class BaseParameter : NotifyPropertyChangedBase
    {
        public event EventHandler<EventArgs> ValueChangedEvent;

        protected virtual void OnValueChanged()
        {
            EventHandler<EventArgs> handler = ValueChangedEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public object Value { get { return GetValue(); }}

        public string Name { get; set; }
        public string Display { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public Type Type { get; set; }
        public string Note { get; set; }

        public abstract string GetValueStr();
        public abstract void SetValueStr(string value);

        public abstract object GetValue();
        public abstract void SetValue(object value);

    }

    public class Parameter<T> : BaseParameter where T : IComparable
    {
        private T _value;
        public new T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value.Equals(_value)) return;
                _value = value;
                OnValueChanged();
                NotifyPropertyChanged("Value");
            }
        }

        public override object GetValue()
        {
            return Value;
        }
        public override string GetValueStr()
        {
            if (typeof(T) == typeof(double))
            {
                var lValue = (long)(Convert.ToDouble(Value) * 100000);
                return lValue.ToString();
            }
            if (typeof (T) == typeof (bool))
            {
                return (bool)(object)Value ? "1" : "0";
            }
            if (typeof (T).IsEnum)
            {
                return ((int)(object) Value).ToString();
            }
            return Value.ToString();
        }

        public override void SetValue(object value)
        {
            Value = (T)value;
        }

        // This should not use the default setter so as not to trigger a write back to micro
        public override void SetValueStr(string value)
        {
            var originalValue = _value;
            try
            {
                if (typeof(T) == typeof(double))
                {
                    var dValue = Double.Parse(value);
                    dValue = dValue / 100000.0;
                    _value = (T)(object)dValue;
                }
                else if (typeof (T) == typeof (bool))
                {
                    var iValue = Int32.Parse(value);
                    _value = (T)(object)(iValue != 0);
                }
                else if (typeof (T).IsEnum)
                {
                    _value = (T)Enum.Parse(typeof (T), value);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    _value = (T)converter.ConvertFromString(value);
                }
            }
            catch (Exception)
            {
                _value = default(T);
            }

            if(!originalValue.Equals(_value))
                NotifyPropertyChanged("Value");
        }
    }

}
