using S7_DMCToolbox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Trending
{

    public class ProfinetModel : Device
    {
        private Model _model = Model.Instance;
        private readonly DateTime[] _lastRequestTimes = new DateTime[4];
        private readonly DateTime[] _lastLogTimes = new DateTime[4];
        private readonly bool[] _newData = new bool[4];
        private readonly bool[] _logPending = new bool[4];

        public List<TagData> TagSamples { get; set; }

        public ProfinetModel()
        {
            BuildDictionaries();
        }

        //Adds a new tag
        //public void AddNewTag()
        //{
        //    TagSamples.Add(new TagData());
        //    TagParameters.Add(new TrendTagParameters(TagParameters.Count + 1));
        //    TagParameters.Last().ParameterChangedEvent += OnParameterChanged;
        //    TagParameters.Last().LocalParameterChangedEvent += OnLocalParameterChanged;
        //    ActualValues.Add(new ActualData() {Current = "0.0000", Max = "0.0000", Min = "0.0000"});
        //}

        public void AddTags(Dictionary<String, Tag> Tags)
        {
            foreach (KeyValuePair<String, Tag> Tag in Tags)
            {
                TagSamples.Add(new TagData() { TagInfo = Tag.Value, Data = new ObservableCollection<DataSample>() });
            }

        }

        private void BuildDictionaries()
        {
            TagSamples = new List<TagData>()
            {

            };

        }

        public void NotifyMeasurementChanged()
        {
            NotifyPropertyChanged("ChannelMeasurements");
        }

        public override void UpdateReadings()
        {

            // Update the readings for the enabled tags.
            foreach (var tag in TagSamples.Where(p => p.TagInfo.Enabled))
            {
                //Keep the length 4 bytes for now so it grabs every data type.
                byte[] Results = _model.ReadBytes(tag.TagInfo.AreaTypeParameter, tag.TagInfo.DbNumber,
                    tag.TagInfo.ByteOffset, 4);
                //Store it into the tagData
                //var tagData = _model.ReceiveLine();
               // ParseDataPacket(ref TagSamples[tag.Tag - 1]);
                //var reading = ActualValues[tag.Tag - 1].Current;
                //var units = TagParameters[tag.Tag - 1].DisplayUnitsParameter.Value.ToString();
                //var time = DateTime.Now;
                //AddNewSample(reading, time, tag.Tag - 1);
                //_model.LogReading(reading, units, time, tag.Tag);
                //TagParameters[tag.Tag - 1].FireLogEvent();
                //_logPending[tag.Tag - 1] = false;
            }
            NotifyMeasurementChanged();


        }

        //Adds a new sample to the data log. We can add it to the TagData.
        public void AddNewSample(string actualvalue, DateTime time, int channelindex)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                //Adds a new sample
                TagSamples[channelindex].Data.Add(new DataSample() {ActualValue = actualvalue, Time = time});
            }));

        }


        //Ask MCU for parameter values
        public override void RefreshParameters()
        {
//            foreach (var channel in ChannelParameters)
//            {
//                foreach (var p in channel.MicroParameters.Values)
//                {
            //Send serial command to MCU asking for parameter value and record response to result
//                    if (!_model.RefreshParameterValue(p, channel.Channel))
//                    {
//                    }
//                }
//                foreach (var p in channel.LocalParameters)
//                {
//                    _model.LoadParameterFromSettings(p, channel.Channel);
//                }
//            }
        }

        public override void RequestLog(int channel)
        {
            _logPending[channel - 1] = true;
        }

        public override void RequestLogAll()
        {
            _logPending[0] = true;
            _logPending[1] = true;
            _logPending[2] = true;
            _logPending[3] = true;
        }

        //Gets int, byte or real for the packet.
        private void ParseDataPacket(ref List<TagData> tagData, DATA_TYPES dataType)
        {
       
        }

        public override void LoadLogFiles()
        {
            //FUTURE USE
            //This method gets called when the directory changes or when a device connects.
            //Search the current directory and load the files for channel 1,2,3 and 4 into ChannelSamples[0],[1],[2] and [3]
//            var dir = new DirectoryInfo(Properties.Settings.Default.workingDirectory);
//            try
//            {
//               if (Directory.Exists(Properties.Settings.Default.workingDirectory))
//                {
//                   foreach (
//                        var file in
            //                      //Search for some meaningful name
            //                        dir.GetFiles("Ch*", SearchOption.TopDirectoryOnly))
            //              {
            //               switch (file.Name)
            //             {
            //                    case "Ch1.csv":
            //                      ChannelSamples[0] = ReadLogFile(file);
            //                    break;
            //              case "Ch2.csv":
            //                ChannelSamples[1] = ReadLogFile(file);
            //              break;
            //        case "Ch3.csv":
            //          ChannelSamples[2] = ReadLogFile(file);
            //        break;
            //  case "Ch4.csv":
            //    ChannelSamples[3] = ReadLogFile(file);
            //  break;
            //  }
            //}                  
            //         }    

            //   }
            //  catch (Exception ex)
            // {
            //   MessageBox.Show("Error Reading From File!\r\n\r\n" + ex.Message);
            // }
            // }
        }
    }
}