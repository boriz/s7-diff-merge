using System;
using System.IO;
using System.Threading;
using MessageBox = System.Windows.MessageBox;

namespace Windows_Trend_View
{
    public abstract class Device : NotifyPropertyChangedBase
    {
        public event EventHandler<ParameterEventArgs> ParameterChangedEvent;
        public event EventHandler<ParameterEventArgs> LocalParameterChangedEvent;

        public abstract void UpdateReadings();
        public abstract void RefreshParameters();
        public abstract void RequestLog(int channel);
        public abstract void RequestLogAll();
        public abstract void LoadLogFiles();

        public TagData ReadLogFile(FileInfo file)
        {
            var channeldata = new TagData();
            foreach (var line in File.ReadLines(file.FullName))
                //Parse the string
            {
                string[] words = line.Split(',');
                channeldata.Data.Add(new DataSample() {ActualValue = words[1], Time = Convert.ToDateTime(words[0])});
            }
            return channeldata;
        }

        protected virtual void OnParameterChanged(object sender, ParameterEventArgs e)
        {
            EventHandler<ParameterEventArgs> handler = ParameterChangedEvent;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnLocalParameterChanged(object sender, ParameterEventArgs e)
        {
            EventHandler<ParameterEventArgs> handler
                = LocalParameterChangedEvent;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

    }

    public class Model : PLCEngine
    {
        private const int UpdateInterval = 20;
        //NLOG FOR FUTURE USE
        //      private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Object _commLock = new Object();
        private bool _runUpdate;
        private Thread _updateReadingThread;


        //Runs continuously and gets data from the PLC.
        private void UpdateReadingProc()
        {
            try
            {
                Thread.Sleep(300);
                while (_runUpdate)
                {
                    lock (_commLock)
                    {
                        ConnectedDevice.UpdateReadings();
                    }
                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException)
            {
                // Do nothing
            }
        }

        public override void Disconnect()
        {
            _runUpdate = false;
            base.Disconnect();
        }

        private static Model _instance;

        public static Model Instance
        {
            get { return _instance ?? (_instance = new Model()); }
        }

        private Device _connectedDevice;

        public Device ConnectedDevice
        {
            get { return _connectedDevice; }
            set
            {
                //LoadFiles() Define this function in the RemoteDisplay in the memory where the logs are.
                //Subscribe to the property change event of the directory. Which will call a method to load the files again.
                _connectedDevice = value;
                Properties.Settings.Default.PropertyChanged += _workingDirectory_PropertyChanged;
                _connectedDevice.ParameterChangedEvent += _connectedDevice_ParameterChangedEvent;
                _connectedDevice.LocalParameterChangedEvent += _connectedDevice_LocalParameterChangedEvent;
                RefreshParameters();
                ConnectedDevice.LoadLogFiles();
                _runUpdate = true;
                _updateReadingThread = new Thread(UpdateReadingProc) {IsBackground = true};
                _updateReadingThread.Start();
            }
        }

        private void _workingDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ConnectedDevice.LoadLogFiles();
        }

        private void _connectedDevice_LocalParameterChangedEvent(object sender, ParameterEventArgs e)
        {
            var channel = sender != null ? (sender as Parameters).Tag : 0;
            SetParameterValueLocal(e.Parameter, channel);
        }

        private void _connectedDevice_ParameterChangedEvent(object sender, ParameterEventArgs e)
        {
            var parameters = sender as Parameters;
            var channel = parameters != null ? parameters.Tag : 0;
            SetParameterValueMicro(e.Parameter, channel);

            // if (DeviceType == DEVICE_TYPE.REMOTE_DISPLAY && e.Parameter.Name == RemoteDisplayParameters.UnitsName)
            {
                //  var rdParameters = parameters as RemoteDisplayParameters;
                //  RefreshParameterValue(rdParameters.AbsoluteOffsetParameter, channel);
                //  RefreshParameterValue(rdParameters.HighToleranceParameter, channel);
                //  RefreshParameterValue(rdParameters.LowToleranceParameter, channel);
                //  RefreshParameterValue(rdParameters.AnalogScaleParameter, channel);
            }
        }

//        public DEVICE_TYPE DeviceType = DEVICE_TYPE.GAGE;
//        public bool IsGage {get { return DeviceType == DEVICE_TYPE.GAGE; }}


        private PLC_TYPE _plcType;

        public PLC_TYPE GageType
        {
            set
            {
                if (value != _plcType)
                {
                    _plcType = value;
                    NotifyPropertyChanged("PLCType");
                }
            }
            get { return _plcType; }
        }


        public void RefreshParameters()
        {
            ConnectedDevice.RefreshParameters();
        }

        public void LoadParameterFromSettings(BaseParameter parm, int channel)
        {
            switch (channel)
            {
                    //case 0:
                    //    parm.SetValueStr(Properties.Ch0.Default[parm.Name].ToString());
                    //    break;
            }
        }

        private void SendParamterReadCommand(string name, int channel = 0)
        {
            //if(IsGage)
            //Send("?" + name + "\r\n");
        }

        public bool SetParameterValueMicro(BaseParameter p, int channel = 0)
        {
            return true;
        }

        private void SendParameterWriteCommand(string name, string value, int channel = 0)
        {
            //if (!IsGage)
            //Send(channelStr + name + "=" + value + "\r\n");
        }

        private void SetParameterValueLocal(BaseParameter p, int channel = 0)
        {
            switch (channel)
            {
                case 0:
                    //   Properties.Ch0.Default[p.Name] = p.GetValueStr();
                    //   Properties.Ch0.Default.Save();
                    break;
            }
        }

        public string GetFirmwareVersion()
        {

            //lock (_commLock)
            //{
            //    Send("Ver\r\n");
            //}
            return "xxx";
        }


        /// <summary>
        /// Called whenever a new device is attached
        /// </summary>
        /// <param name="recentPort">com port where new PLC is located e.g. "COM4"</param>
        public bool NewDevice(string Address, int rack, int slot)
        {
            //Log event
            // Logger.Info("Device Attached");

            //Connect PLCEngine to device
            if (Connect(Address, rack, slot))
            {

                //Update log and details
                //   Logger.Info("Device successfully connected on [" + Address + "]");
                //Logger.Info("Device version: " + Version);
                return true;
            }
            return false;
        }

        private void SaveParameters()
        {
            //Send("Save\r\n");
            // Wait for flash write to finish
            //Thread.Sleep(300);
        }

        internal void RequestLogAll()
        {
            //ConnectedDevice.RequestLogAll();
        }


        public void RequestLog(int channel)
        {
            ConnectedDevice.RequestLog(channel);
        }

        public void ThreadSafeSend(string message)
        {
            lock (_commLock)
            {
//                Send(message);
            }
        }

        public void LogReading(string reading, string units, DateTime time, int channel)
        {
            // try
            {
                //      if (!Directory.Exists(Properties.Settings.Default.workingDirectory))
                //          Directory.CreateDirectory(Properties.Settings.Default.workingDirectory);
                //      var info =
                //              time.ToString("yyyy-MM-dd hh:mm:ss.ff") + "," +
                //             //DateTime.Now.ToString("hh:mm:ss") + "," +
                //            reading + "," +
                //           units + "\r\n";
                //  File.AppendAllText(Properties.Settings.Default.workingDirectory + "Ch" + channel + ".csv", info);
                //}
                //catch (Exception ex)
                // {
                //    MessageBox.Show("Error Logging to File!\r\n\r\n" + ex.Message);
                //   logger.ErrorException("Error Logging to File", ex);
                // }
            }


        }
    }
}