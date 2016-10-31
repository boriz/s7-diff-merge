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
using DotNetSiemensPLCToolBoxLibrary.Source;
using OfficeOpenXml;
using System.Windows;


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
        private string WinCCPortalDigitalAlarmsUDTFilePath;
        private int currentExportId;
        private bool _comfortSelected;
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
        public string WinCCPortalDigitalAlarmsImportFilePath { get; set; }
        public string WinCCPortalDigitalAlarmsExportFilePath { get; set; }
        public string KepwarePortalImportFilePath { get; set; }
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

            AddChildrenToKepwareExportTable(exportTable, blk.Structure.Children, CurrentBlock.Value.SymbolicName, blk);
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
                if (!SymbolTable.ContainsKey(sym.Symbol))
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

        internal void ExportKepwareAllBlocks()
        {
            DoJob(new ThreadStart(ExportKepwareAllBlocksAsync));
        }

        internal void ExportKepwarePortal()
        {
            DoJob(new ThreadStart(ExportKepwarePortalAsync));
        }

        internal void ExportKepwareAllBlocksAsync()
        {

            //this function is where the actual code is to export 1 block. Need to replace with exporting all global blocks to one CSV.
          ExportTable.KepwareExportTableDataTable exportTable = new ExportTable.KepwareExportTableDataTable(); //this is a table that holds every data block.
          foreach(KeyValuePair<string,Block> db in AllBlocks) //so this is not looking for everything with "db" in it, but it's making a new array of blocks from a
              //all it is doing is indexing a collection. in this case AllBlocks is a collection of KeyValuePairs. So it is not filtering anything by DB or FB or FC or anything. I ti is just indexing AllB
              //indexing AllBlcoks and putting the contents one at a time into the variable "db"
           {
                if (!(db.Value.Name.ToLower().StartsWith("db"))) //check to make sure a data block is selected
                    //Then here, we are checking the block to see if its ia DB or FB or FC. ok  If tit's not a DB, go to the next block. Sorry it's hard to type over remote haha ok and that's why you changed return to the continue. yes.
                {
                    continue; //this will skip the block in the for loop if it is not a datablock
                }
            
           
                S7DataBlock blk = (S7DataBlock)db.Value.BlockContents; //grab block contents
                if (blk.IsInstanceDB)
                {
                    continue;
                }
                ExportTable.KepwareExportTableDataTable singleBlockTable = new ExportTable.KepwareExportTableDataTable(); //create new CSV table

                AddChildrenToKepwareExportTable(singleBlockTable, blk.Structure.Children, db.Value.SymbolicName, db.Value.BlockContents); //get addresses for all items in data block

                foreach (ExportTable.KepwareExportTableRow row in singleBlockTable.Rows) //add every row from this data block to our global table
                {
                    exportTable.Rows.Add(row.ItemArray);//.AddKepwareExportTableRow(row);
                }
            }
          AddSymbolsToKepwareExportTable(exportTable);
          CreateKepwareCSVFromDataTable(exportTable); //Export to CSV file  from our global table

            //so it kinda looks like you could loop through all the blocks and add them all to exportTable, and then call the CreateKepwareCSVFromDataTable.
            //I set up the basics so you now have a new button on the application that will link to whatever you put in this function.

            //This code is just to create aa "list" of recently exported blocks for easier use next time. Is not relevent to our task of exporting all blocks:
            //if (!Properties.Settings.Default.RecentlyUsedBlocks.Contains(CurrentBlock.Key))
            //{
            //    Properties.Settings.Default.RecentlyUsedBlocks.Insert(0, CurrentBlock.Key);
            //    while (Properties.Settings.Default.RecentlyUsedBlocks.Count > 150)
            //    {
            //        Properties.Settings.Default.RecentlyUsedBlocks.RemoveAt(150);
            //    }
            //    Properties.Settings.Default.Save();
            //}

        }

        internal void ExportKepwareAsync()
        {
            if (!(CurrentBlock.Value.Name.ToLower().StartsWith("db")))
            {
                return;
            }
            
            S7DataBlock blk = (S7DataBlock)CurrentBlock.Value.BlockContents;
            ExportTable.KepwareExportTableDataTable exportTable = new ExportTable.KepwareExportTableDataTable();

            AddChildrenToKepwareExportTable(exportTable, blk.Structure.Children, CurrentBlock.Value.SymbolicName, blk);
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

        internal void ExportKepwarePortalAsync()
        {
            if (!File.Exists(KepwarePortalImportFilePath))
            {
                return;
            }

            ExportTable.KepwareExportTableDataTable exportTable = new ExportTable.KepwareExportTableDataTable();

            var alarmDataBlocks = DbSourceParser.ParseDBSourceFile(KepwarePortalImportFilePath);
            currentExportId = 0;
            foreach (S7DataBlock dataBlock in alarmDataBlocks.Values)
            {
                AddChildrenToKepwareExportTable(exportTable, dataBlock.Structure.Children, dataBlock.Name, dataBlock);
            }

            CreateKepwareCSVFromDataTable(exportTable);
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

        private void AddChildrenToKepwareExportTable(ExportTable.KepwareExportTableDataTable exportTable, List<S7DataRow> Children, String ParentPath, S7Block blk, int ByteAdder = 0)
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
                            newRow.Address = "DB" + blk.BlockNumber + ".DBX" + ByteAddress + "." + BitAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.BYTE:
                            newRow.Data_Type = "Byte";
                            newRow.Address = "DB" + blk.BlockNumber + ".DBB" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.DINT:
                        case S7DataRowType.DWORD:
                        case S7DataRowType.TIME:
                            newRow.Data_Type = "DWord";
                            newRow.Address = "DB" + blk.BlockNumber + ".DBD" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.WORD:
                            newRow.Data_Type = "Word";
                            newRow.Address = "DB" + blk.BlockNumber + ".DBW" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.INT:
                            newRow.Data_Type = "Short";
                            newRow.Address = "DB" + blk.BlockNumber + ".DBW" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.REAL:
                            newRow.Data_Type = "FLOAT";
                            newRow.Address = "DB" + blk.BlockNumber + ".DBD" + ByteAddress;
                            exportTable.AddKepwareExportTableRow(newRow);
                            break;
                        case S7DataRowType.STRING:
                            newRow.Data_Type = "String";
                            newRow.Address = "DB" + blk.BlockNumber + ".String" + ByteAddress + "." + (child.ByteLength - 2);
                            exportTable.AddKepwareExportTableRow(newRow);

                            break;
                        
                        case S7DataRowType.FB:
                        case S7DataRowType.BLOCK_FB:
                            AddChildrenToKepwareExportTable(exportTable, child.Children, newRow.Tag_Name, blk, ByteAdder);
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
                                newRow.Address = "DB" + blk.BlockNumber + ".DBX" + ByteAddress + "." + BitAddress;
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
                                newRow.Address = "DB" + blk.BlockNumber + ".DBB" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.DINT:
                            case S7DataRowType.DWORD:
                            case S7DataRowType.TIME:
                                newRow.Data_Type = "DWord";
                                newRow.Address = "DB" + blk.BlockNumber + ".DBD" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.WORD:
                                newRow.Data_Type = "Word";
                                newRow.Address = "DB" + blk.BlockNumber + ".DBW" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.INT:
                                newRow.Data_Type = "Short";
                                newRow.Address = "DB" + blk.BlockNumber + ".DBW" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.REAL:
                                newRow.Data_Type = "FLOAT";
                                newRow.Address = "DB" + blk.BlockNumber + ".DBD" + ByteAddress;
                                exportTable.AddKepwareExportTableRow(newRow);
                                break;
                            case S7DataRowType.STRING:
                                newRow.Data_Type = "String";
                                newRow.Address = "DB" + blk.BlockNumber + ".String" + ByteAddress + "." + (child.ByteLength - 2);
                                exportTable.AddKepwareExportTableRow(newRow);

                                break;
                            case S7DataRowType.UDT:
                            case S7DataRowType.FB:
                            case S7DataRowType.BLOCK_FB:
                            case S7DataRowType.STRUCT:
                                AddChildrenToKepwareExportTable(exportTable, child.Children, ParentPath + "." + child.Name + "[" + i + "]", blk, (i - child.ArrayStart.First()) * (child.ByteLength / (child.ArrayStop.First() - child.ArrayStart.First() + 1)) + ByteAdder);
                                break;
                        }

                    }
                }
            }
        }

        private void AddSymbolsToKepwareExportTable(ExportTable.KepwareExportTableDataTable exportTable)
        {
            foreach (KeyValuePair<String, Tag> child in Symbols)
            {
                if (!child.Value.Address.StartsWith("M"))
                    continue;

                ExportTable.KepwareExportTableRow newRow = exportTable.NewKepwareExportTableRow();
                newRow.Respect_Data_Type = "1";
                newRow.Client_Access = "R/W";
                newRow.Scan_Rate = "100";
                newRow.Address = child.Value.Address;
                newRow.Tag_Name = child.Key;
                newRow.Description = child.Value.Name;
                //newRow.Tag_Name = ParentPath + "." + child.Name;
                //newRow.Description = child.Comment;
                //int BitAddress = child.BlockAddress.BitAddress;
                //int ByteAddress = child.BlockAddress.ByteAddress + ByteAdder;

                switch (child.Value.Address.Substring(0, 2).ToUpper())
                {
                    case "M ":
                        newRow.Data_Type = "Boolean";
                        exportTable.AddKepwareExportTableRow(newRow);
                        break;
                    case "MB":
                        newRow.Data_Type = "Byte";
                        exportTable.AddKepwareExportTableRow(newRow);
                        break;
                    case "MD":
                        newRow.Data_Type = "FLOAT";
                        exportTable.AddKepwareExportTableRow(newRow);
                        break;
                    case "MW":
                        newRow.Data_Type = "Short";
                        exportTable.AddKepwareExportTableRow(newRow);
                        break;
                    default:
                        break;
                }
            }
            
        }

        #endregion

        #region Export AlarmWorX

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

            AddChildrenToAlarmworxExportTable(exportTable, blk.Structure.Children, CurrentBlock.Value.SymbolicName, "");
            File.WriteAllText(AlarmWorxExportFilePath, CreateAlarmWorxCSVFromDataTable(exportTable).ToString());
        }

        internal void ExportAlarmWorxPortal(string importFilePath, string exportFilePath)
        {
            if (!Properties.Settings.Default.RecentOPCServers.Contains(SelectedOPCServer))
            {
                Properties.Settings.Default.RecentOPCServers.Insert(0, SelectedOPCServer);
                while (Properties.Settings.Default.RecentOPCServers.Count > 15)
                {
                    Properties.Settings.Default.RecentOPCServers.RemoveAt(15);
                }
            }

            Properties.Settings.Default.Save();

            ExportTable.AlarmWorxExportTableDataTable exportTable = new ExportTable.AlarmWorxExportTableDataTable();

            var alarmDataBlocks = DbSourceParser.ParseDBSourceFile(importFilePath);
            currentExportId = 0;
            foreach (S7DataBlock dataBlock in alarmDataBlocks.Values)
            {
                AddChildrenToAlarmworxExportTable(exportTable, dataBlock.Structure.Children, dataBlock.Name, "");
            }
            
            File.WriteAllText(exportFilePath, CreateAlarmWorxCSVFromDataTable(exportTable).ToString());
        }

        private StringBuilder CreateAlarmWorxCSVFromDataTable(ExportTable.AlarmWorxExportTableDataTable exportTable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"#AWX_Source;");
            IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().
                                              Select(p => p.ColumnName);// => column.ColumnName.Replace("_", " "));
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in exportTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            return sb;
        }

        private void AddChildrenToAlarmworxExportTable(ExportTable.AlarmWorxExportTableDataTable exportTable, List<S7DataRow> Children, string blockName, string strCommentPath)
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
                            newRow.Input1 = "\"" + SelectedOPCServer + "." + blockName + "." + child.StructuredName + "\"";
                            newRow.BaseText = "\"" + strCommentPath.Trim() + " " + child.Comment.Trim() + "\"";  // Message text 
                            newRow.DIG_MsgText = " ";   // Prevents 'Digital Alarm' text at the end of each message
                            newRow.DIG_Limit = "1";     // Alarm state value needs to be 1 for a digital
                            newRow.DIG_Severity = "500"; // Default severity is 500
                            newRow.DIG_RequiresAck = "1"; // Require an acknowledge by default
                            exportTable.AddAlarmWorxExportTableRow(newRow);
                            break;
                        case S7DataRowType.UDT:
                        case S7DataRowType.STRUCT:
                            // Build comments path string, separate each level by the space
                            AddChildrenToAlarmworxExportTable(exportTable, child.Children, blockName, strCommentPath.Trim() + " " + child.Comment.Trim());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region WinCC Portal Alarms Export

        internal void ExportWinCCPortalDigitalAlarms(bool comfortSelected)
        {
            _comfortSelected = comfortSelected;
            DoJob(new ThreadStart(ExportWinCCPortalAlarmsAsync));
        }

        internal void ExportWinCCPortalAlarmsAsync()
        {
            if (!File.Exists(WinCCPortalDigitalAlarmsImportFilePath))
            {
                return;
            }

            ExportTable.WinCCPortalDigitalAlarmsExportTableDataTable exportTable = new ExportTable.WinCCPortalDigitalAlarmsExportTableDataTable();

            var alarmDataBlocks = DbSourceParser.ParseDBSourceFile(WinCCPortalDigitalAlarmsImportFilePath);
            currentExportId = 0;
            foreach (S7DataBlock AlarmDataBlock in alarmDataBlocks.Values)
            {
                AddDataBlockWinCCPortalDigitalAlarmsExportTable(exportTable, AlarmDataBlock.Structure.Children, AlarmDataBlock.Name, new List<String>(), new List<String>());
            }

            WritePortalXLSXFromDataTable(WinCCPortalDigitalAlarmsExportFilePath, exportTable);


        }

        private void WritePortalXLSXFromDataTable(String excelPath, ExportTable.WinCCPortalDigitalAlarmsExportTableDataTable exportTable)
        {
            using (ExcelPackage ep = new ExcelPackage())
            {
                ep.Workbook.Worksheets.Add("DiscreteAlarms");

                int col = 1;
                int row = 1;

                IEnumerable<string> columnNames = exportTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);

                foreach (String column in columnNames)
                {
                    ep.Workbook.Worksheets[1].Cells[row, col].Value = column;
                    col++;
                }
                foreach (DataRow tableRow in exportTable.Rows)
                {
                    row++;
                    col = 1;
                    foreach (object tableColumn in tableRow.ItemArray)
                    {
                        ep.Workbook.Worksheets[1].Cells[row, col].Value = tableColumn.ToString();
                        col++;
                    }
                }

                Byte[] epByteArray = ep.GetAsByteArray();
                try
                {
                    File.WriteAllBytes(excelPath, epByteArray);
                }
                catch
                {
                    MessageBox.Show("\"" + Path.GetFileName(excelPath) + "\" is already open or something.\n\rI couldn't make alarms like you wanted :(", "Uh oh...", MessageBoxButton.OK);
                }

            }

        }

        private void AddDataBlockWinCCPortalDigitalAlarmsExportTable(ExportTable.WinCCPortalDigitalAlarmsExportTableDataTable exportTable, List<S7DataRow> children, String datablockName, List<String> structNames, List<String> structComments)
        {
            foreach (S7DataRow child in children)
            {

                ExportTable.WinCCPortalDigitalAlarmsExportTableRow newRow = exportTable.NewWinCCPortalDigitalAlarmsExportTableRow();

                switch (child.DataType)
                {
                    case S7DataRowType.BOOL:
                        currentExportId++;
                        newRow.ID = currentExportId.ToString();
                        newRow.Name = "Discrete_alarm_" + currentExportId.ToString();

                        newRow.Class = "Errors";
                        newRow._FieldInfo__Alarm_text_ = "";
                        if (_comfortSelected)
                        {
                            newRow.Trigger_tag = "dbErrors";
                            newRow.Trigger_bit = AddressToTriggerBit(child.BlockAddress.ByteAddress, child.BlockAddress.BitAddress);
                        }
                        else
                        {
                            String tag = datablockName + ".";
                            foreach (String s in structNames) { tag = tag + s + "."; }
                            tag = tag + child.Name;
                            tag = "\"" + tag + "\"";

                            newRow.Trigger_tag = tag;
                            newRow.Trigger_bit = "0";
                        }


                        newRow.Acknowledgement_tag = "<No value>";
                        newRow.Acknowledgement_bit = "0";

                        newRow.PLC_Acknowledgement_tag = "<No value>";
                        newRow.PLC_Acknowledgement_bit = "0";

                        newRow.Group = "<No value>";
                        newRow.Report = "False";

                        if (structNames.Count > 0)
                        {
                            String comment = "";
                            foreach (String s in structComments) { comment = comment + s + " - "; }
                            newRow._Alarm_text__en_US___Alarm_text = comment + child.Comment;
                            newRow._Info_text__en_US___Info_text = comment + child.Comment;
                        }
                        else
                        {
                            newRow._Alarm_text__en_US___Alarm_text = child.Comment;
                            newRow._Info_text__en_US___Info_text = child.Comment;
                        }

                        exportTable.AddWinCCPortalDigitalAlarmsExportTableRow(newRow);
                        break;
                    case S7DataRowType.UDT:
                    case S7DataRowType.STRUCT:
                        structComments.Add(child.Comment);
                        structNames.Add(child.Name);
                        AddDataBlockWinCCPortalDigitalAlarmsExportTable(exportTable, child.Children, datablockName, structNames, structComments);
                        break;
                    default:
                        break;
                }
            }
            if (structNames.Count > 0)
            {
                structNames.RemoveAt(structNames.Count - 1);
            }
            if (structComments.Count > 0)
            {
                structComments.RemoveAt(structComments.Count - 1);
            }
        }

        private String AddressToTriggerBit(int byteAddress, int bitAddress)
        {

            int triggerBit;

            if (byteAddress % 2 == 0)
            {
                triggerBit = (byteAddress + 1) * 8 + bitAddress;
            }
            else
            {
                triggerBit = (byteAddress - 1) * 8 + bitAddress;
            }

            return triggerBit.ToString();
        }

        #endregion

    }
}


 