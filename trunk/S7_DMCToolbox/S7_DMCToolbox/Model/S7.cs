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
        private Boolean _IsBusy = false;
        private Int16 _ProgressBarCurrent;
        private Int16 _ProgressBarMax;
        
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
     
        #endregion

        #region Private Command Implementations
        internal void GetBlocksAsync()
        {

            Project proj = Projects.LoadProject(_ProjectPath, false);
            if (proj == null) //project not found
                return;
            List<ProjectBlockInfo> allBlockInfo = GetBlocksFromProject(proj.ProjectStructure.SubItems);
            Dictionary<String, Block> myBlocks = new Dictionary<String, Block>();

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
                    
                    for (int i=child.ArrayStart.First();i <= child.ArrayStop.Last();i++)
                    {
                        AddChildrenToKepwareExportTable(exportTable, child.Children, ParentPath + "." + child.Name + "[" + i + "]", blk, (i - child.ArrayStart.First()) * (child.ByteLength / (child.ArrayStop.First() - child.ArrayStart.First() + 1)));
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

            AddChildrenToAlarmworxExportTable(exportTable, blk.Structure.Children);
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

        private void AddChildrenToAlarmworxExportTable(ExportTable.AlarmWorxExportTableDataTable exportTable, List<S7DataRow> Children)
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
                            newRow.BaseText = "\"" + child.Parent.Parent.Comment + " " + child.Parent.Comment + " " + child.Comment + "\"";
                            exportTable.AddAlarmWorxExportTableRow(newRow);
                            break;
                        case S7DataRowType.UDT:
                        case S7DataRowType.STRUCT:
                            AddChildrenToAlarmworxExportTable(exportTable, child.Children);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

       
    }
}


 