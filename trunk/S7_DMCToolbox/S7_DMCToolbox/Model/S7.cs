using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using DMCBase;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System.Text.RegularExpressions;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using System.Data;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using S7_DMCToolbox.Model;


namespace S7_DMCToolbox
{
    class S7 : NotifyPropertyChangedBase
    {
        #region Private Variables
        private String _ProjectPath;
        private Boolean _IsCanceling = false;
        private String _ProjectName;
        private Dictionary<String, Block> _AllBlocks;
        private Dictionary<String, Tag> _AllTags;
        private Dictionary<String, Tag> _TrendTags = new Dictionary<string,Tag>();
        private Boolean _IsBusy = false;
        private Int16 _ProgressBarCurrent;
        private Int16 _ProgressBarMax;
        private Dictionary<String, Tag> _Symbols;
        #endregion

        #region Public Properties
        public String ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }
       
        public KeyValuePair<String, Block> CurrentBlock { get; set; }
        public KeyValuePair<String, Tag> CurrentTag { get; set; }
        public String ProjectPath
        {
            get
            {
                if (_ProjectPath == null)
                {
                    ProjectPath = Properties.Settings.Default.ProjectPath;
                }
                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
                if ((value.ToLower().EndsWith(".s7p")) || (value.ToLower().EndsWith(".s7l")) || (value.ToLower().EndsWith(".zip")))
                {
                    GetBlocks();
                }
                NotifyPropertyChanged("ProjectPath");
            }
        }
        public String SelectedOPCServer { get; set; }
        public String SelectedAlarmFolder { get; set; }
        public string AlarmWorxExportFilePath { get; set; }
        public string KepwareExportFilePath { get; set; }
        public string WinCCFlexDigitalAlarmsExportFilePath { get; set; }
        public string AllBlocksExportFilePath { get; set; }

        public Dictionary<String, Block> AllBlocks
        {
            get
            {
                if (_AllBlocks == null)
                {
                    _AllBlocks = new Dictionary<String, Block>();
                }
                return _AllBlocks;
            }
            set
            {
                _AllBlocks = value;
                NotifyPropertyChanged("AllBlocks");
            }
        }
        public Dictionary<String,Tag> AllTags
        {
            get
            {
                if (_AllTags == null)
                {
                    _AllTags = new Dictionary<String, Tag>();
                }
                return _AllTags;
            }
            set
            {
                _AllTags = value;
                NotifyPropertyChanged("AllTags");
            }
        }
        public Dictionary<String,Tag> Symbols
        {
            get
            {
                if (_Symbols == null)
                {
                    _Symbols = new Dictionary<String,Tag>();
                }
                return _Symbols;
            }
            set
            {
                _Symbols = value;
                NotifyPropertyChanged("Symbols");
            }
        }
        public Dictionary<String, Tag> TrendTags
        {
            get
            {
                if (_TrendTags == null)
                {
                    _TrendTags = new Dictionary<String, Tag>();
                }
                return _TrendTags;
            }
            set
            {
                _TrendTags = value;
                NotifyPropertyChanged("TrendTags");
            }
        }
        public Boolean IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                _IsBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }
        public Int16 ProgressBarCurrent
        {
            get
            {
                return _ProgressBarCurrent;
            }
            set
            {
                _ProgressBarCurrent = value;
                NotifyPropertyChanged("ProgressBarCurrent");
            }
        }
        public Int16 ProgressBarMax
        {
            get
            {
                return _ProgressBarMax;
            }
            set
            {
                _ProgressBarMax = value;
                NotifyPropertyChanged("ProgressBarMax");
            }
        }
        #endregion

        #region Model Initialization and Unloading
        public S7()
        {
      //      SimaticInit();
        }
       
        internal void ClearTempDirectories()
        {
            /*
             *  private String _LeftSourceFilePath = Path.GetTempPath() + Guid.NewGuid() + "\\";
                private String _RightSourceFilePath = Path.GetTempPath() + Guid.NewGuid() + "\\";
                private String _MergedSourceFilePath = Path.GetTempPath() + Guid.NewGuid() + "\\";
                private String _ExtractZipProjectPath = Path.GetTempPath() + Guid.NewGuid() + "\\";
             */
       
            //if (Directory.Exists(_LeftSourceFilePath)) Directory.Delete(_LeftSourceFilePath, true);
            //if (Directory.Exists(_RightSourceFilePath)) Directory.Delete(_RightSourceFilePath, true);
            //if (Directory.Exists(_MergedSourceFilePath)) Directory.Delete(_MergedSourceFilePath, true);
            //if (Directory.Exists(_ExtractLeftProjectPath)) Directory.Delete(_ExtractLeftProjectPath, true);
            //if (Directory.Exists(_ExtractRightProjectPath)) Directory.Delete(_ExtractRightProjectPath, true);
        }
        #endregion

        #region Public Command Functions
        internal void GetBlocks()
        {
            //GetBlocksAsync();
 	        DoJob(new ThreadStart(GetBlocksAsync));
         //   NotifyPropertyChanged("Blocks");
        }
        internal void GetAllTags()
        {
            //GetBlocksAsync();
            DoJob(new ThreadStart(GetAllTagsAsync));
            //   NotifyPropertyChanged("Blocks");
        }
        internal void AddTrendTag()
        {
            //GetBlocksAsync();
            DoJob(new ThreadStart(AddTrendTagAsync));
            //   NotifyPropertyChanged("Blocks");
        }
        internal void AddTrendTagAsync()
        {
            if (!_TrendTags.Contains(CurrentTag))
                _TrendTags.Add(CurrentTag.Key, CurrentTag.Value);
            TrendTags = new Dictionary<string,Tag>(_TrendTags);
            NotifyPropertyChanged("TrendTags");
        }
        #endregion

        #region Private Command Implementations
        internal void GetAllTagsAsync()
        {
            if (CurrentBlock.Value.Name.Equals("Symbols"))
            {
                AllTags = Symbols;
                return;
            }
            else if (!(CurrentBlock.Value.Name.ToLower().StartsWith("db")))
            {
                return;
            }
            

            S7DataBlock blk = (S7DataBlock)CurrentBlock.Value.BlockContents;
            ExportTable.KepwareExportTableDataTable exportTable = new ExportTable.KepwareExportTableDataTable();

            AddChildrenToKepwareExportTable(exportTable, blk.Structure.Children, CurrentBlock.Value.SymbolicName, CurrentBlock.Value);
            Dictionary<String, Tag> myTags = new Dictionary<string,Tag>();
            foreach (ExportTable.KepwareExportTableRow Row in exportTable)
            {
                Regex LastNumberInString = new Regex("(\\d+)(?!.*\\d)");
                Tag NewTag = new Tag() { Name = Row.Tag_Name, AreaTypeParameter = Trending.AREA_TYPE.DB};
                
                String[] SplitAddress = Row.Address.Split('.');
                if (SplitAddress.Length == 2)
                    NewTag.BitOffset = 0;
                else
                    NewTag.BitOffset = int.Parse(LastNumberInString.Match(Row.Address).Value);

                NewTag.ByteOffset = int.Parse(LastNumberInString.Match(Row.Address.Split('.')[1]).Value);
                NewTag.DbNumber = int.Parse(LastNumberInString.Match(Row.Address.Split('.')[0]).Value);
                
                NewTag.Address = Row.Address;
                myTags.Add(Row.Tag_Name, NewTag);

            }
            AllTags = myTags;
        }
        internal void GetBlocksAsync()
        {
            Project proj = null;
            try
            {
                proj = Projects.LoadProject(_ProjectPath, false);
            }
            catch (Exception ex)
            {
                // TODO: HAndle exception gracefully
                proj = null;
            }

            if (proj == null) //project not found
                return;
            
            List<ProjectBlockInfo> allBlockInfo = GetBlocksFromProject(proj.ProjectStructure.SubItems);
            Dictionary<String, Block> myBlocks = new Dictionary<String, Block>();
            Symbols = GetSymbolTableFromProject(proj.ProjectStructure.SubItems);
            myBlocks.Add("Symbols", new Block() {Name="Symbols", SymbolicName = "Symbols" });

            foreach (ProjectBlockInfo item in allBlockInfo)
            {
                if ((item != null) && (item.BlockType != PLCBlockType.SDB) && (item.BlockType != PLCBlockType.SFB) && (item.BlockType != PLCBlockType.SFC))
                {
                    try
                    {
                        DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block blk = (DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block)item.GetBlock();
                        myBlocks.Add(item.ParentFolder.ToString().Replace("(zipped)", "") + "\\" + blk.BlockName, new Block()
                        {
                            Modified = blk.LastCodeChange,
                            Name = blk.BlockName,
                            Size = blk.CodeSize,
                            SymbolicName = Regex.Match(item.ToString(), @"(?<=\().+?(?=\))").Value,
                            BlockContents = blk
                        });
                    }
                    catch (Exception e)
                    {
                        //TODO
                    }
                    
                }
            }
            
            AllBlocks = myBlocks;
            NotifyPropertyChanged("AllBlocks");
        }
        private List<ProjectBlockInfo> GetBlocksFromProject(List<ProjectFolder> folders)
        {
            //create new list
            List<ProjectBlockInfo> allBlocks = new List<ProjectBlockInfo>();

            foreach (ProjectFolder fldr in folders)
            {
                if ((fldr is IBlocksFolder) && !(fldr is SourceFolder))
                {
                    IBlocksFolder offlineBlocks = (IBlocksFolder)fldr;
                    allBlocks = offlineBlocks.readPlcBlocksList();
                }
                if ((fldr.SubItems != null) && (fldr.SubItems.Count > 0))
                {
                    allBlocks.AddRange(GetBlocksFromProject(fldr.SubItems));
                }
            }

            return allBlocks;
        }

        private Dictionary<String,Tag> GetSymbolTableFromProject(List<ProjectFolder> folders)
        {
            //create new list
            List<SymbolTableEntry> s = GetSymbolEntries(folders);
            Dictionary<String,Tag> SymbolTable = new Dictionary<string,Tag>();

            foreach (SymbolTableEntry sym in s)
            {
                Tag NewTag = new Tag();
                NewTag.Name = sym.Symbol;
                NewTag.Address = sym.Operand;
                switch (sym.Operand.First())
                {
                    case 'I':
                        NewTag.AreaTypeParameter = Trending.AREA_TYPE.IN;
                        break;
                    case 'M':
                        NewTag.AreaTypeParameter = Trending.AREA_TYPE.MEMORY;
                        break;
                    case 'Q':
                        NewTag.AreaTypeParameter = Trending.AREA_TYPE.OUT;
                        break;
                    default:
                        NewTag.AreaTypeParameter = Trending.AREA_TYPE.DB;
                        break;
                }
                Regex LastNumberInString = new Regex("(\\d+)(?!.*\\d)");
               
                String[] SplitAddress = sym.Operand.Split('.');
                if (SplitAddress.Length == 1)
                {
                    NewTag.BitOffset = 0;
                    NewTag.ByteOffset = int.Parse(LastNumberInString.Match(sym.Operand).Value);
                }
                else
                {
                    NewTag.BitOffset = int.Parse(LastNumberInString.Match(sym.Operand).Value);
                    NewTag.ByteOffset = int.Parse(LastNumberInString.Match(sym.Operand.Split('.')[0]).Value);
                }

                SymbolTable.Add(sym.Symbol, NewTag);
            }

            return SymbolTable;
            
        }
        private List<SymbolTableEntry> GetSymbolEntries(List<ProjectFolder> folders)
        {
            //create new list
            List<SymbolTableEntry> s = new List<SymbolTableEntry>();

            foreach (ProjectFolder fldr in folders)
            {
                if (fldr is ISymbolTable)
                {
                    s.AddRange(((SymbolTable)fldr).SymbolTableEntrys);
                }
                if ((fldr.SubItems != null) && (fldr.SubItems.Count > 0))
                {
                    s.AddRange(GetSymbolEntries(fldr.SubItems));
                }

            }
            return s;
        }

        #endregion

     
        #region Multi-threaded Helpers
        private Thread jobThread;
        private Thread workerThread;

        private void DoJob(ThreadStart workerThreadStart)
        {
            jobThread = new Thread(JobThread);
       //     jobThread.SetApartmentState(ApartmentState.STA);
            jobThread.Start(workerThreadStart);
        }

        private void JobThread(object workerThreadStart)
        {
            if (!(workerThreadStart is ThreadStart))
            {
                throw new ArgumentException("Parameter must be of type ThreadStart", "workerThreadStart");
            }

            ProgressBarCurrent = 0;
            ProgressBarMax = 1;

            workerThread = new Thread(workerThreadStart as ThreadStart);
            //workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();

            IsBusy = true;
            // Wait for worker thread to finish
            workerThread.Join();
            IsBusy = false;
           
        }

        #endregion


        #region WinCC Flexible Digital Alarms

        internal void ExportWinCCFlexDigitalAlarms()
        {
            DoJob(new ThreadStart(ExportWinCCFlexDigitalAlarmsAsync));
        }

        internal void ExportWinCCFlexDigitalAlarmsAsync()
        {
            if (!(CurrentBlock.Value.Name.ToLower().StartsWith("db")))
            {
                return;
            }

            S7DataBlock blk = (S7DataBlock)CurrentBlock.Value.BlockContents;
            ExportTable.WinCCFlexDigitalAlarmsExportTableDataTable exportTable = new ExportTable.WinCCFlexDigitalAlarmsExportTableDataTable();

            AddChildrenWinCCFlexDigitalAlarmsExportTable(exportTable, blk.Structure.Children, "");
            CreateWinCCFlexDigitalAlarmsCSVFromDataTable(exportTable);
        }

        private void CreateWinCCFlexDigitalAlarmsCSVFromDataTable(ExportTable.WinCCFlexDigitalAlarmsExportTableDataTable exportTable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"// WinCC flexible 2008 SP3 Advanced V 1.4.0.0 (1.16.16)");
            sb.AppendLine(@"// Automatically generated alarm export file.");
            sb.AppendLine(@"// " + DateTime.Now.ToString("M/d/yyyy h:m:s tt"));
            sb.AppendLine(@"// @V1.0.0");
            sb.AppendLine(@"");

            //sb.AppendLine(@"//Alarm type,Alarm number,Alarm class,Trigger tag,Trigger bit number,Acknowledgment HMI tag,Acknowledgment HMI tag bit number,Acknowledgment PLC tag,Acknowledgment PLC tag bit number,Alarm group,Reported,Text[en-US],Field info[01],Infotext[en-US]");            
            IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName.Replace("_", " "));
            sb.AppendLine(@"//" + string.Join("\t", columnNames));

            sb.AppendLine(@"");

            // Append all rows
            foreach (DataRow row in exportTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join("\t", fields));
            }

            File.WriteAllText(WinCCFlexDigitalAlarmsExportFilePath, sb.ToString());
        }

        private void AddChildrenWinCCFlexDigitalAlarmsExportTable(ExportTable.WinCCFlexDigitalAlarmsExportTableDataTable exportTable, List<S7DataRow> Children, string strCommentPath)
        {
            foreach (S7DataRow child in Children)
            {
                if (!child.IsArray)
                {
                    ExportTable.WinCCFlexDigitalAlarmsExportTableRow newRow = exportTable.NewWinCCFlexDigitalAlarmsExportTableRow();
                    
                    // Alarm type,Alarm number,Alarm class,Trigger tag,Trigger bit number,Acknowledgment HMI tag,Acknowledgment HMI tag bit number,Acknowledgment PLC tag,Acknowledgment PLC tag bit number,Alarm group,Reported,Text[en-US],Field info[01],Infotext[en-US]
                    // D,8,Alarms,dbErrors,8,,,,,,0,en-US= Global Sheeting E-Stop button #01 (HW reset required),,en-US= Global Sheeting E-Stop button #01 (HW reset required)

                    switch (child.DataType)
                    {
                        case S7DataRowType.BOOL:
                            newRow.AlarmType = "\"" + "D" + "\"";

                            // Calculate trigger bit/alarm number
                            int iWinCC_BitNumber;
                            if (child.BlockAddress.ByteAddress % 2 == 0) 
                            {
                                iWinCC_BitNumber = (child.BlockAddress.ByteAddress + 1) * 8 + child.BlockAddress.BitAddress;
                            }
                            else
                            {
                                iWinCC_BitNumber = (child.BlockAddress.ByteAddress - 1) * 8 + child.BlockAddress.BitAddress;
                            }
                            newRow.TriggerBitNumber = "\"" + iWinCC_BitNumber.ToString() + "\"";
                            newRow.AlarmNumber = "\"" + (child.BlockAddress.ByteAddress * 16 + child.BlockAddress.BitAddress + 1).ToString() + "\"";

                            newRow.AlarmClass = "\"" + "Alarms" + "\"";
                            newRow.TriggerTag = "\"" + CurrentBlock.Value.SymbolicName + "\"";
                            
                            newRow.Text = "\"" + "en-US=" + strCommentPath.Trim() + " " + child.Comment.Trim() + "\"";  // Message text
                            newRow.Infotext = "\"" + "en-US=" + strCommentPath.Trim() + " " + child.Comment.Trim() + "\"";

                            exportTable.AddWinCCFlexDigitalAlarmsExportTableRow(newRow);
                            break;
                        case S7DataRowType.UDT:
                        case S7DataRowType.STRUCT:
                            AddChildrenWinCCFlexDigitalAlarmsExportTable(exportTable, child.Children, strCommentPath.Trim() + " " + child.Comment.Trim());
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        #endregion


        #region All Blocks Export

        internal void ExportAllBlocks()
        {
            DoJob(new ThreadStart(ExportAllBlocksAsync));
        }

        internal void ExportAllBlocksAsync()
        {
            ExportTable.AllBlocksExportTableDataTable exportTable = new ExportTable.AllBlocksExportTableDataTable();

            AddChildrenExportAllBlocksExportTable(exportTable, AllBlocks);
            CreateExportAllBlocksCSVFromDataTable(exportTable);
        }

        private void CreateExportAllBlocksCSVFromDataTable(ExportTable.AllBlocksExportTableDataTable exportTable)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName.Replace("_", " "));
            sb.AppendLine(string.Join("\t", columnNames));

            // Append all rows
            var rows = from ds in exportTable orderby ds.Path select ds;
            foreach (var row in rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join("\t", fields));
            }

            byte[] bom = new byte[3];
            bom[0] = 0xEF;
            bom[1] = 0xBB;
            bom[2] = 0xBF;
            File.WriteAllBytes(AllBlocksExportFilePath, bom);
            File.WriteAllText(AllBlocksExportFilePath, sb.ToString());
        }

        private void AddChildrenExportAllBlocksExportTable(ExportTable.AllBlocksExportTableDataTable exportTable, Dictionary<String, Block> AllBlocks)
        {
            foreach (string key in AllBlocks.Keys)
            {
                Block blk = AllBlocks[key];
                // Fill block parameters
                if (blk.Name.ToLower().StartsWith("db"))                
                {
                    S7DataBlock lblk = (S7DataBlock)blk.BlockContents;
                    ExportTable.AllBlocksExportTableRow newRow = exportTable.NewAllBlocksExportTableRow();

                    newRow.Number = "\"" + blk.Name + "\"";
                    newRow.Name = "\"" + blk.SymbolicName + "\"";
                    newRow.Path = "\"" + key + "\"";
                    newRow.Type = "\"" + "DB" + "\"";
                    newRow.Language = "\"" + "STL" + "\"";

                    newRow.Comment = "\"";
                    if (lblk.Title != null && lblk.Title != "")
                    {
                        newRow.Comment = "\"" + ClearString(lblk.Title) + "\"";
                    }
                    newRow.Comment = newRow.Comment + "\"";

                    newRow.Size = "\"" + blk.Size.ToString() + "\"";

                    if (lblk.Version != null)
                    {
                        newRow.Version = "\"" + lblk.Version + "\"";
                    }
                    else
                    {
                        newRow.Version = "\"0.0\"";
                    }

                    newRow.LastModified = "\"" + blk.Modified.ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                    exportTable.AddAllBlocksExportTableRow(newRow);                    
                } 
                else if ( blk.Name.ToLower().StartsWith("fb") || blk.Name.ToLower().StartsWith("fc") || blk.Name.ToLower().StartsWith("ob") )
                {
                    S7FunctionBlock lblk = (S7FunctionBlock)blk.BlockContents;
                    ExportTable.AllBlocksExportTableRow newRow = exportTable.NewAllBlocksExportTableRow();

                    newRow.Number = "\"" + blk.Name + "\"";
                    newRow.Name = "\"" + blk.SymbolicName + "\"";
                    newRow.Path = "\"" + key + "\"";
                    newRow.Type = "\"" + lblk.BlockType.ToString() + "\"";
                    newRow.Language = "\"" + lblk.BlockLanguage.ToString() + "\"";

                    newRow.Comment = "\"";
                    if (lblk.Title != null && lblk.Title != "")
                    {
                        newRow.Comment = newRow.Comment + ClearString(lblk.Title);
                    }
                    if (lblk.Description != null && lblk.Description != "")
                    {
                        newRow.Comment = newRow.Comment + ". " + ClearString(lblk.Description);
                    }
                    newRow.Comment = newRow.Comment + "\"";

                    newRow.Size = "\"" + blk.Size + "\"";

                    if (lblk.Version != null)
                    {
                        newRow.Version = "\"" + lblk.Version + "\"";
                    }
                    else
                    {
                        newRow.Version = "\"0.0\"";
                    }

                    newRow.LastModified = "\"" + blk.Modified.ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                    exportTable.AddAllBlocksExportTableRow(newRow);
                }
            }
        }


        private string ClearString(string str)
        {
            string ret = str.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "").Replace(@"//", "");
            Regex reg;

            reg = new Regex(@"\-+");
            ret = reg.Replace(ret, "-");

            reg = new Regex(@"\*+");
            ret = reg.Replace(ret, "*");

            return ret;
        }

        #endregion

        #region Export Kepware

        internal void ExportKepware()
        {
            DoJob(new ThreadStart(ExportKepwareAsync));
        }

        internal void ExportKepwareAsync()
        {
            if (!(CurrentBlock.Value.Name.ToLower().StartsWith("db")))
            {
                return;
            }
            
            S7DataBlock blk = (S7DataBlock)CurrentBlock.Value.BlockContents;
            ExportTable.KepwareExportTableDataTable exportTable = new ExportTable.KepwareExportTableDataTable();

            AddChildrenToKepwareExportTable(exportTable, blk.Structure.Children, CurrentBlock.Value.SymbolicName, CurrentBlock.Value);
            CreateKepwareCSVFromDataTable(exportTable);

            if (!Properties.Settings.Default.RecentlyUsedBlocks.Contains(CurrentBlock.Key))
            {
                Properties.Settings.Default.RecentlyUsedBlocks.Insert(0, CurrentBlock.Key);
                while (Properties.Settings.Default.RecentlyUsedBlocks.Count > 150)
                {
                    Properties.Settings.Default.RecentlyUsedBlocks.RemoveAt(150);
                }
                Properties.Settings.Default.Save();
            }
            
        }

        private void CreateKepwareCSVFromDataTable(ExportTable.KepwareExportTableDataTable exportTable)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName.Replace("_", " "));
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in exportTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(KepwareExportFilePath, sb.ToString());
        }

        private void AddChildrenToKepwareExportTable(ExportTable.KepwareExportTableDataTable exportTable, List<S7DataRow> Children, String ParentPath, Block blk, int ByteAdder = 0)
        {
            foreach (S7DataRow child in Children)
            {
                if (!child.IsArray)
                {
                    ExportTable.KepwareExportTableRow newRow = exportTable.NewKepwareExportTableRow();
                    newRow.Respect_Data_Type = "1";
                    newRow.Client_Access = "R/W";
                    newRow.Scan_Rate = "100";
                    newRow.Address = child.BlockAddress.ToString();
                    newRow.Tag_Name = ParentPath + "." + child.Name;
                    newRow.Description = child.Comment;
                    int BitAddress = child.BlockAddress.BitAddress;
                    int ByteAddress = child.BlockAddress.ByteAddress + ByteAdder;

                    switch (child.DataType)
                    {
                        case S7DataRowType.BOOL:
                            newRow.Data_Type = "Boolean";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBX" + ByteAddress + "." + BitAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.BYTE:
                            newRow.Data_Type = "Byte";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBB" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.DINT:
                        case S7DataRowType.DWORD:
                        case S7DataRowType.TIME:
                            newRow.Data_Type = "DWord";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBD" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.WORD:
                            newRow.Data_Type = "Word";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBW" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.INT:
                            newRow.Data_Type = "Short";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBW" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.REAL:
                            newRow.Data_Type = "FLOAT";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBD" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.STRING:
                            newRow.Data_Type = "String";
                            newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".String" + ByteAddress + "." + (child.ByteLength - 2);
                            exportTable.AddKepwareExportTableRow(newRow);

                            break;
                        case S7DataRowType.UDT:
                        case S7DataRowType.STRUCT:
                            AddChildrenToKepwareExportTable(exportTable, child.Children, newRow.Tag_Name, blk, ByteAdder);
                            break;
                    }
                }
                else //IsArray = true
                {
                    List<S7DataRow> arrayList = new List<S7DataRow>();
                    int ArrayBitAdder = 0;
                    int ArrayByteAdder = 0;
                    for (int i=child.ArrayStart.First();i <= child.ArrayStop.Last();i++)
                    {
                        ExportTable.KepwareExportTableRow newRow = exportTable.NewKepwareExportTableRow();
                        newRow.Respect_Data_Type = "1";
                        newRow.Client_Access = "R/W";
                        newRow.Scan_Rate = "100";
                        newRow.Address = child.BlockAddress.ToString();
                        newRow.Tag_Name = ParentPath + "." + child.Name + "[" + i + "]";
                        newRow.Description = child.Comment;
                        int BitAddress = child.BlockAddress.BitAddress + ArrayBitAdder;
                        int ByteAddress = child.BlockAddress.ByteAddress + ByteAdder + ArrayByteAdder + (i - child.ArrayStart.First()) * (child.ByteLength / (child.ArrayStop.First() - child.ArrayStart.First() + 1));
                        switch (child.DataType)
                        {
                            case S7DataRowType.BOOL:
                                newRow.Data_Type = "Boolean";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBX" + ByteAddress + "." + BitAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                ArrayBitAdder++;
                                if (ArrayBitAdder == 8)
                                {
                                    ArrayBitAdder = 0;
                                    ArrayByteAdder++;
                                }
                                break;
                            case S7DataRowType.BYTE:
                                newRow.Data_Type = "Byte";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBB" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.DINT:
                            case S7DataRowType.DWORD:
                            case S7DataRowType.TIME:
                                newRow.Data_Type = "DWord";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBD" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.WORD:
                                newRow.Data_Type = "Word";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBW" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.INT:
                                newRow.Data_Type = "Short";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBW" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.REAL:
                                newRow.Data_Type = "FLOAT";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".DBD" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.STRING:
                                newRow.Data_Type = "String";
                                newRow.Address = "DB" + blk.BlockContents.BlockNumber + ".String" + ByteAddress + "." + (child.ByteLength - 2);
                                exportTable.AddKepwareExportTableRow(newRow);

                                break;
                            case S7DataRowType.UDT:
                            case S7DataRowType.STRUCT:
                                AddChildrenToKepwareExportTable(exportTable, child.Children, ParentPath + "." + child.Name + "[" + i + "]", blk, (i - child.ArrayStart.First()) * (child.ByteLength / (child.ArrayStop.First() - child.ArrayStart.First() + 1)) + ByteAdder);
                                break;
                        }

                    }
                }
            }
        }

        internal void ExportAlarmWorx()
        {
            if (!Properties.Settings.Default.RecentOPCServers.Contains(SelectedOPCServer))
            {
                Properties.Settings.Default.RecentOPCServers.Insert(0, SelectedOPCServer);
                while (Properties.Settings.Default.RecentOPCServers.Count > 15)
                {
                    Properties.Settings.Default.RecentOPCServers.RemoveAt(15);
                }
            }
            if (!Properties.Settings.Default.RecentAlarmFolderNames.Contains(SelectedAlarmFolder))
            {
                Properties.Settings.Default.RecentAlarmFolderNames.Insert(0, SelectedAlarmFolder);
                while (Properties.Settings.Default.RecentAlarmFolderNames.Count > 15)
                {
                    Properties.Settings.Default.RecentAlarmFolderNames.RemoveAt(15);
                }
            }
            if (!Properties.Settings.Default.RecentlyUsedBlocks.Contains(CurrentBlock.Key))
            {
                Properties.Settings.Default.RecentlyUsedBlocks.Insert(0, CurrentBlock.Key);
                while (Properties.Settings.Default.RecentlyUsedBlocks.Count > 150)
                {
                    Properties.Settings.Default.RecentlyUsedBlocks.RemoveAt(150);
                }
                Properties.Settings.Default.Save();
            }

            Properties.Settings.Default.Save();

            S7DataBlock blk = (S7DataBlock)CurrentBlock.Value.BlockContents;
            ExportTable.AlarmWorxExportTableDataTable exportTable = new ExportTable.AlarmWorxExportTableDataTable();

            AddChildrenToAlarmworxExportTable(exportTable, blk.Structure.Children, "");
            CreateAlarmWorxCSVFromDataTable(exportTable);
        }

        private void CreateAlarmWorxCSVFromDataTable(ExportTable.AlarmWorxExportTableDataTable exportTable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"#AWX_Source;");
            IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName.Replace("_", " "));
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in exportTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(AlarmWorxExportFilePath, sb.ToString());
        }

        private void AddChildrenToAlarmworxExportTable(ExportTable.AlarmWorxExportTableDataTable exportTable, List<S7DataRow> Children, string strCommentPath)
        {
            foreach (S7DataRow child in Children)
            {
                if (!child.IsArray)
                {
                    ExportTable.AlarmWorxExportTableRow newRow = exportTable.NewAlarmWorxExportTableRow();
                    switch (child.DataType)
                    {
                        case S7DataRowType.BOOL:
                            newRow.LocationPath = "\"" +  @"\\Alarm Configurations\" + SelectedAlarmFolder + "\"";
                            newRow.Name = "\"" +  SelectedAlarmFolder + "." + child.StructuredName + "\"";
                            newRow.Description = "\"" + child.Comment + "\"";
                            newRow.LastModified = DateTime.Now;
                            newRow.Input1 = "\"" + SelectedOPCServer + "." + CurrentBlock.Value.SymbolicName + "." + child.StructuredName + "\"";
                            newRow.BaseText = "\"" + strCommentPath.Trim() + " " + child.Comment.Trim() + "\"";  // Message text 
                            newRow.DIG_MsgText = " ";   // Prevents 'Digital Alarm' text at the end of each message
                            exportTable.AddAlarmWorxExportTableRow(newRow);
                            break;
                        case S7DataRowType.UDT:
                        case S7DataRowType.STRUCT:
                            // Build comments path string, separate each level by the space
                            AddChildrenToAlarmworxExportTable(exportTable, child.Children, strCommentPath.Trim() + " " + child.Comment.Trim());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

    }
}


 