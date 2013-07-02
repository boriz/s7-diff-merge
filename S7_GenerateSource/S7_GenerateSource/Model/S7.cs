using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimaticLib;
using System.Collections.ObjectModel;
using NLog;
using S7HCOM_XLib;
using System.Collections;
using DMCBase;
using System.Threading;
using System.IO;
using System.Diagnostics;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;


namespace S7_GenerateSource
{
    class S7 : NotifyPropertyChangedBase
    {

        #region Private Variables
        private String _LeftProjectPath;
        private String _RightProjectPath;

        

        private S7Project _LeftProject;
        private S7Project _RightProject;
        
        private String _LeftSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sL_";
        private String _RightSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sR_";
        private String _MergedSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sM_";

        private String _ExtractLeftProjectPath = "";
        private String _ExtractRightProjectPath = "";

        private Dictionary<String, Block> _LeftBlocks;
        private Dictionary<String, Block> _RightBlocks;
        private Dictionary<String, Blocks> _AllBlocks;

        private Boolean _IsBusy = false;
        private Int16 _ProgressBarCurrent;
        private Int16 _ProgressBarMax;

        private SimaticCache _dicLeftSimaticCache = new SimaticCache();
        private SimaticCache _dicRightSimaticCache = new SimaticCache();

        private String _StatusBarText = "Ready";

        #endregion

        #region Public Properties

        private ObservableCollection<LogEvent> _LogModel;
        public ObservableCollection<LogEvent> LogModel
        {
            get
            {
                return _LogModel;
            }

            set
            {
                _LogModel = value;
            }
        }

        public KeyValuePair<String, Blocks> CurrentBlock { get; set; }

        public String LeftProjectPath
        {
            get
            {
                if (_LeftProjectPath == null)
                {
                    LeftProjectPath = Properties.Settings.Default.LeftProjectPath;
                }
                return _LeftProjectPath;
            }
            set
            {               
                _LeftProjectPath = value;
                NotifyPropertyChanged("LeftProjectPath");
            }
        }

        public String RightProjectPath
        {
            get
            {
                if (_RightProjectPath == null)
                {
                    RightProjectPath = Properties.Settings.Default.RightProjectPath;
                }
                return _RightProjectPath;
            }
            set
            {
                _RightProjectPath = value;               
                NotifyPropertyChanged("RightProjectPath");
            }
        }

        public String ExtractLeftProjectPath
        {
            get
            {
                return _ExtractLeftProjectPath;
            }
            set
            {
                _ExtractLeftProjectPath = value;
                NotifyPropertyChanged("ExtractLeftProjectPath");
            }
        }

        public String ExtractRightProjectPath
        {
            get
            {
                return _ExtractRightProjectPath;
            }
            set
            {
                _ExtractRightProjectPath = value;
                NotifyPropertyChanged("ExtractRightProjectPath");
            }
        }

        public Dictionary<String, Block> LeftBlocks
        {
            get
            {
                if (_LeftBlocks == null)
                {
                    _LeftBlocks = new Dictionary<String, Block>();
                }
                
                return _LeftBlocks;
                
            }
            set
            {
                _LeftBlocks = value;
            }
        }

        public Dictionary<String, Block> RightBlocks
        {
            get
            {
                if (_RightBlocks == null)
                {
                    _RightBlocks = new Dictionary<String, Block>();
                }
                return _RightBlocks;
            }
            set
            {
                _RightBlocks = value;
            }
        }

        public Dictionary<String, Blocks> AllBlocks
        {
            get
            {
                if (_AllBlocks == null)
                {
                    _AllBlocks = new Dictionary<String, Blocks>();
                }
                return _AllBlocks;
            }
            set
            {
                _AllBlocks = value;
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

        public String StatusBarText
        {
            get
            {
                return _StatusBarText;
            }

            set
            {
                _StatusBarText = value;
                NotifyPropertyChanged("StatusBarText");
            }
        }

        #endregion

        #region Model Initialization and Unloading
        
        public S7()
        {
            ClearTempDirectories();
            SimaticInit();
        }

        private void SimaticInit()
        {
            //_Simatic = new Simatic();
        }

        internal void ClearTempDirectories()
        {
            try
            {
                //TODO
                //if (_Simatic != null)
                //{
                //    _Simatic.Close();
                //    _Simatic = null;
                //}

                DeleteFolder(ExtractLeftProjectPath);
                DeleteFolder(ExtractRightProjectPath);
                DeleteFolder(Path.GetTempPath() + "DMC_Merge");
            }
            catch (Exception ex)
            {
                EventFire.Error("S7.ClearTempDirectories. Exception: " + ex.ToString());
            }
        }
        #endregion


        #region Public Command Functions

        internal void GetBlocks()
        {
            DoJob(new ThreadStart(() =>
            {
                // User forced to re-load projects 
                ExtractRightProjectPath = "";
                ExtractLeftProjectPath = "";
                GetBlocksAsync(_RightProjectPath, _LeftProjectPath);
            }));
        }


        internal void Save()
        {
            DoJob(new ThreadStart(() =>
            {
                if (_ExtractRightProjectPath != "")
                {
                    EventFire.Info("Archiving.");
                    saveZippedProject(_ExtractRightProjectPath, _RightProjectPath);
                    EventFire.Info("Archive complete.");
                }
            }));
        }


        [STAThread]
        internal void CopyBlockToRight()
        {
            DoJob(new ThreadStart(CopyBlockToRightAsync));
        }
        

        [STAThread]
        internal void StartDiffProcess()
        {
            
            DoJob(new ThreadStart(StartDiffProcessAsync));
        }

        #endregion


        #region Private Command Implementations
        
        internal void GetBlocksAsync(string strRightProjectPath, string strLeftProjectPath)
        {
            try
            {
                EventFire.Info("Loading blocks. Begin.");

                // Trying to find a project name from the path
                strRightProjectPath = GetProjectFileNameFromPath(strRightProjectPath);
                strLeftProjectPath = GetProjectFileNameFromPath(strLeftProjectPath);
                Step7ProjectV5 left = new Step7ProjectV5(strLeftProjectPath, false);
                Step7ProjectV5 right = new Step7ProjectV5(strRightProjectPath, false);

                //grab blocks from projects
                List<ProjectBlockInfo> blkPojectLeft = GetBlocksFromProject(left.ProjectStructure);
                List<ProjectBlockInfo> blkPojectRight = GetBlocksFromProject(right.ProjectStructure);

                LeftBlocks.Clear();
                RightBlocks.Clear();

                EventFire.Info("Loading blocks. Parsing projects.");
                foreach (ProjectBlockInfo item in blkPojectLeft)
                {
                    if (item != null)
                    {
                        DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block blk = (DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block)item.GetBlock();
                        LeftBlocks.Add(item.ParentFolder.ToString().Replace("(zipped)", "") + "\\" + blk.BlockName, CreateNewBlock(item));
                    }
                }

                foreach (ProjectBlockInfo item in blkPojectRight)
                {
                    //if ((item != null) && (item.BlockType != PLCBlockType.SDB) && (item.BlockType != PLCBlockType.SFB) && (item.BlockType != PLCBlockType.SFC))
                    if (item != null)
                    {
                        DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block blk = (DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block)item.GetBlock();
                        RightBlocks.Add(item.ParentFolder.ToString().Replace("(zipped)", "") + "\\" + blk.BlockName, CreateNewBlock(item));
                    }
                }

                // Add symbol tables
                List<ISymbolTable> symLeft = GetSymbolTablesFromProject(left.ProjectStructure);
                List<ISymbolTable> symRight = GetSymbolTablesFromProject(right.ProjectStructure);

                foreach (ISymbolTable item in symLeft)
                {
                    LeftBlocks.Add(item.ToString().Replace("(zipped)", "") + "\\" + item.Name, new Block()
                    {
                        Name = item.Name,
                        SymbolicName = "Symbol Table",
                        Type = PLCBlockType.AllBlocks,
                        TypeString = "",
                        Language = PLCLanguage.SYM,
                        LanguageString = "SYM",
                        Modified = item.LastChange,
                        Size = item.SymbolTableEntrys.Count
                    });
                }

                foreach (ISymbolTable item in symRight)
                {
                    RightBlocks.Add(item.ToString().Replace("(zipped)", "") + "\\" + item.Name, new Block()
                    {
                        Name = item.Name,
                        SymbolicName = "Symbol Table",
                        Type = PLCBlockType.AllBlocks,
                        TypeString = "",
                        Language = PLCLanguage.SYM,
                        LanguageString = "SYM",
                        Modified = item.LastChange,
                        Size = item.SymbolTableEntrys.Count
                    });
                }

                // Add HMIs
                foreach (HMIFolder item in left.HMIFolders)
                {
                    LeftBlocks.Add(item.ToString().Replace("(zipped)", "") + "\\" + item.Name, new Block()
                    {
                        Name = item.Type.ToString(),    // HMI type
                        SymbolicName = item.Name,       // Full station name
                        Type = PLCBlockType.AllBlocks,
                        TypeString = "",
                        Language = PLCLanguage.HMI,
                        LanguageString = "HMI",
                        Modified = item.Modified,
                        Size = null
                    });
                }

                foreach (HMIFolder item in right.HMIFolders)
                {
                    RightBlocks.Add(item.ToString().Replace("(zipped)", "") + "\\" + item.Name, new Block()
                    {
                        Name = item.Type.ToString(),    // HMI type
                        SymbolicName = item.Name,       // Full station name
                        Type = PLCBlockType.AllBlocks,
                        TypeString = "",
                        Language = PLCLanguage.HMI,
                        LanguageString = "HMI",
                        Modified = item.Modified,
                        Size = null
                    });
                }

                // Add HW configs
                foreach (ProjectFolder item in left.ProjectStructure.SubItems)
                {
                    if (item.GetType() == typeof(StationConfigurationFolder))
                    {
                        StationConfigurationFolder it = (StationConfigurationFolder)item;
                        LeftBlocks.Add(it.ToString().Replace("(zipped)", "") + "\\" + it.Name, new Block()
                        {
                            Name = "HW Config",    // HW config type
                            SymbolicName = it.Name,       // Full station name
                            Type = PLCBlockType.AllBlocks,
                            TypeString = "",
                            Language = PLCLanguage.HWC,
                            LanguageString = "HWC",
                            Modified = it.HW_Config_Modified,
                            Size = (int)it.HW_Config_Size
                        });
                    }
                }

                foreach (ProjectFolder item in right.ProjectStructure.SubItems)
                {
                    if (item.GetType() == typeof(StationConfigurationFolder))
                    {
                        StationConfigurationFolder it = (StationConfigurationFolder)item;
                        RightBlocks.Add(it.ToString().Replace("(zipped)", "") + "\\" + it.Name, new Block()
                        {
                            Name = "HW Config",    // HW config type
                            SymbolicName = it.Name,       // Full station name
                            Type = PLCBlockType.AllBlocks,
                            TypeString = "",
                            Language = PLCLanguage.HWC,
                            LanguageString = "HWC",
                            Modified = it.HW_Config_Modified,
                            Size = (int)it.HW_Config_Size
                        });
                    }
                }

                // Add orpah blocks
                foreach (String key in LeftBlocks.Keys)
                {
                    if (!RightBlocks.Keys.Contains(key))
                    {
                        RightBlocks.Add(key, new Block()
                        {
                            Name = LeftBlocks[key].Name,
                            SymbolicName = "-------NOT FOUND-------",
                            Type = PLCBlockType.AllBlocks,
                            TypeString = "",
                            Language = PLCLanguage.UNDEF,
                            LanguageString = ""
                        });
                        LeftBlocks[key].NameSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].SizeSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].SymbolicNameSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].ModifiedSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].NameSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].SizeSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].SymbolicNameSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].ModifiedSimilarity = BlockSimilarityType.Orphan;
                    }
                }

                foreach (String key in RightBlocks.Keys)
                {
                    if (!LeftBlocks.Keys.Contains(key))
                    {
                        LeftBlocks.Add(key, new Block()
                        {
                            Name = RightBlocks[key].Name,
                            SymbolicName = "-------NOT FOUND-------",
                            Type = PLCBlockType.AllBlocks,
                            TypeString = "",
                            Language = PLCLanguage.UNDEF,
                            LanguageString = ""
                        });
                        LeftBlocks[key].NameSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].SizeSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].SymbolicNameSimilarity = BlockSimilarityType.Orphan;
                        LeftBlocks[key].ModifiedSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].NameSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].SizeSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].SymbolicNameSimilarity = BlockSimilarityType.Orphan;
                        RightBlocks[key].ModifiedSimilarity = BlockSimilarityType.Orphan;
                    }
                }

                foreach (String key in RightBlocks.Keys)
                {
                    if (RightBlocks[key].SizeSimilarity != BlockSimilarityType.Orphan)
                    {
                        RightBlocks[key].NameSimilarity = RightBlocks[key].Name.Equals(LeftBlocks[key].Name) ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        RightBlocks[key].SizeSimilarity = RightBlocks[key].Size == LeftBlocks[key].Size ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        RightBlocks[key].SymbolicNameSimilarity = RightBlocks[key].SymbolicName.Equals(LeftBlocks[key].SymbolicName) ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        RightBlocks[key].ModifiedSimilarity = RightBlocks[key].Modified == LeftBlocks[key].Modified ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        LeftBlocks[key].NameSimilarity = RightBlocks[key].Name.Equals(LeftBlocks[key].Name) ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        LeftBlocks[key].SizeSimilarity = RightBlocks[key].Size == LeftBlocks[key].Size ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        LeftBlocks[key].SymbolicNameSimilarity = RightBlocks[key].SymbolicName.Equals(LeftBlocks[key].SymbolicName) ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                        LeftBlocks[key].ModifiedSimilarity = RightBlocks[key].Modified == LeftBlocks[key].Modified ? BlockSimilarityType.Identical : BlockSimilarityType.Different;
                    }
                }

                Dictionary<String, Blocks> newAllBlocks = new Dictionary<string, Blocks>();
                foreach (String key in RightBlocks.Keys)
                {
                    newAllBlocks.Add(key, new Blocks() { LeftBlock = LeftBlocks[key], RightBlock = RightBlocks[key] });
                }
                AllBlocks = newAllBlocks;

                // Assuming we got a s7 v5 project - close them
                ((DotNetSiemensPLCToolBoxLibrary.Projectfiles.Step7ProjectV5)left).Dispose();
                ((DotNetSiemensPLCToolBoxLibrary.Projectfiles.Step7ProjectV5)right).Dispose();

                NotifyPropertyChanged("LeftBlocks");
                NotifyPropertyChanged("RightBlocks");
                NotifyPropertyChanged("AllBlocks");

                EventFire.Info("Loading blocks. Process completed.");
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        private Block CreateNewBlock(ProjectBlockInfo item)
        {
            
            try
            {
                DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block blk = (DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7Block)item.GetBlock();
                if (blk.BlockType == PLCBlockType.SourceBlock)
                {
                    // Special logic for source block
                    DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7SourceBlock srcBlk = (DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5.S7SourceBlock)blk;
                    return new BlockSource()
                        {
                            Modified = srcBlk.LastCodeChange,
                            Name = srcBlk.BlockName,
                            Type = srcBlk.BlockType,
                            TypeString = srcBlk.BlockType.ToString(),
                            Language = srcBlk.BlockLanguage,
                            LanguageString = srcBlk.BlockLanguage.ToString(),
                            Size = srcBlk.CodeSize,
                            SymbolicName = Regex.Match(item.ToString(), @"(?<=\().+?(?=\))").Value,
                            SourceText = srcBlk.Text,
                            Filename = srcBlk.Filename
                        };
                }
                else
                {
                    // Not a source block.
                    return new Block()
                        {
                            Modified = blk.LastCodeChange,
                            Name = blk.BlockName,
                            Type = blk.BlockType,
                            TypeString = blk.BlockType.ToString(),
                            Language = blk.BlockLanguage,
                            LanguageString = blk.BlockLanguage.ToString(),
                            Size = blk.CodeSize,
                            SymbolicName = Regex.Match(item.ToString(), @"(?<=\().+?(?=\))").Value
                        };
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
            return null;
        }

        private String GetProjectFileNameFromPath(String strPath)
        {
            try
            {
                if (strPath.ToLower().EndsWith(".s7p"))
                {
                    return strPath;
                }
                else if (strPath.ToLower().EndsWith(".zip"))
                {
                    return strPath;
                }
                else
                {
                    // This is probably folder name
                    if (Directory.Exists(strPath))
                    {
                        var Projects = Directory.EnumerateFiles(strPath, "*.s7p", SearchOption.AllDirectories);
                        if (Projects.Count() > 0)
                        {
                            return Projects.First();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            return strPath;
        }

        private List<ProjectBlockInfo> GetBlocksFromProject(ProjectFolder project)
        {
            //create new list
            List<ProjectBlockInfo> allBlocks = new List<ProjectBlockInfo>();

            try
            {
                if (project.GetType() == typeof(BlocksOfflineFolder))
                {
                    BlocksOfflineFolder offline = (BlocksOfflineFolder)project;
                    allBlocks.AddRange(offline.BlockInfos);
                }

                if (project.GetType() == typeof(SourceFolder))
                {
                    SourceFolder source = (SourceFolder)project;
                    // TODO: Use source files instead of the compiled FBs?
                    allBlocks.AddRange(source.BlockInfos);
                }

                //add all child blocks
                foreach (ProjectFolder subFolder in project.SubItems)
                {
                    allBlocks.AddRange(GetBlocksFromProject(subFolder));
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            return allBlocks;
        }

        private List<ISymbolTable> GetSymbolTablesFromProject(ProjectFolder project)
        {
            //create new list
            List<ISymbolTable> allSymTables = new List<ISymbolTable>();
            try
            {
                if (project.GetType() == typeof(S7ProgrammFolder))
                {
                    ISymbolTable symtab = ((S7ProgrammFolder)project).SymbolTable;
                    allSymTables.Add(symtab);
                }

                //add all child blocks
                foreach (ProjectFolder subFolder in project.SubItems)
                {
                    allSymTables.AddRange(GetSymbolTablesFromProject(subFolder));
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            return allSymTables;
        }


        [STAThread]
        internal void StartDiffProcessAsync()
        {
            Simatic _Simatic;
            try
            {
                IsBusy = true;

                //_Simatic = new Simatic();

                ProgressBarMax = 10;
                ProgressBarCurrent = 1;

                DateTime tsSearchTimeStart = DateTime.Now;
                EventFire.Info("Merging. Search start timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                if (CurrentBlock.Value.LeftBlock.ModifiedSimilarity == BlockSimilarityType.Orphan)
                {
                    EventFire.Error("Merging. Can't merge orphan blocks.");
                    return;
                }
                _Simatic = new Simatic();

                if (_Simatic == null)
                {
                    EventFire.Info("Merging. Creating Simatic instance.");
                    _Simatic = new Simatic();
                }
                ProgressBarCurrent++;

                // Attaching both projects
                LoadingBothSimaticProjects(ref _Simatic);

                // Special logic to merge source files
                if (CurrentBlock.Value.LeftBlock.Type == PLCBlockType.SourceBlock && CurrentBlock.Value.RightBlock.Type == PLCBlockType.SourceBlock)
                {
                    S7Source rightBlock = null;
                    S7Source leftBlock = null;
                    rightBlock = FindCurrentSourceInProgram(_RightProject, ref _dicRightSimaticCache);
                    leftBlock = FindCurrentSourceInProgram(_LeftProject, ref _dicLeftSimaticCache);
                    // TODO: wrap it into function
                    // Find left and right blocks at the same time                
                    //Thread thRight = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Merging Source. Searching for the right block in Simatic structure.");
                    //    rightBlock = FindCurrentSourceInProgram(_RightProject.Programs, ref _dicRightSimaticCache);
                    //    EventFire.Info("Merging Source. Right block found.");
                    //}));
                    //thRight.Start();

                    //Thread thLeft = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Merging Source. Searching for the left block in Simatic structure.");
                    //    leftBlock = FindCurrentSourceInProgram(_LeftProject.Programs, ref _dicLeftSimaticCache);
                    //    EventFire.Info("Merging Source. Left block found.");
                    //}));
                    //thLeft.Start();

                    //// Wait for both threads to complete
                    //thRight.Join();
                    //thLeft.Join();
                    //// TODO: end wrap it into function

                    ProgressBarCurrent++;

                    EventFire.Info("Merging. Search complete timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                    S7_BlockCopyMerge mrg = new S7_BlockCopyMerge();
                    mrg.MergeBlocks(leftBlock, rightBlock, ExtractRightProjectPath, CurrentBlock);
                }
                else
                {
                    // Standard FB/FC/DB

                    S7Block leftBlock = null;
                    S7Block rightBlock = null;
                    rightBlock = FindCurrentBlockInProgram(_RightProject, ref _dicRightSimaticCache);
                    leftBlock = FindCurrentBlockInProgram(_LeftProject, ref _dicLeftSimaticCache);
                    // TODO: wrap it into function
                    // Find left and right blocks at the same time                
                    //Thread thRight = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Merging. Searching for the right block in Simatic structure.");
                    //    rightBlock = FindCurrentBlockInProgram(_RightProject.Programs, ref _dicRightSimaticCache);
                    //    EventFire.Info("Merging. Right block found.");
                    //}));
                    //thRight.Start();


                    //Thread thLeft = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Merging. Searching for the left block in Simatic structure.");
                    //    leftBlock = FindCurrentBlockInProgram(_LeftProject.Programs, ref _dicLeftSimaticCache);
                    //    EventFire.Info("Merging. Left block found.");
                    //}));
                    //thLeft.Start();

                    //// Wait for both threads to complete
                    //thRight.Join();
                    //thLeft.Join();
                    // TODO: end wrap it into function

                    ProgressBarCurrent++;

                    EventFire.Info("Merging. Search complete timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                    S7_BlockCopyMerge mrg = new S7_BlockCopyMerge();
                    mrg.MergeBlocks(leftBlock, rightBlock);

                }
                ProgressBarCurrent++;

                // TODO: Remove temp files

                // Refresh list
                EventFire.Info("Merging. Refreshing projects.");
                GetBlocksAsync(ExtractRightProjectPath, ExtractLeftProjectPath);

                ProgressBarCurrent = ProgressBarMax;

                EventFire.Info("Merging. Process completed.");
                IsBusy = false;
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
            
        }


        [STAThread]
        internal void CopyBlockToRightAsync()
        {
            Simatic _Simatic = null;
            try
            {
                if (CurrentBlock.Key.Count() <= 0)
                {
                    return;
                }

                IsBusy = true;
                ProgressBarMax = 10;
                ProgressBarCurrent = 1;

                DateTime tsSearchTimeStart = DateTime.Now;
                EventFire.Info("Copying. Search start timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                if (_Simatic == null)
                {
                    EventFire.Info("Copying. Creating Simatic instance.");
                    _Simatic = new Simatic();
                }

                // Trying to find loaded projects
                LoadingBothSimaticProjects(ref _Simatic);

                // Special logic to copy source files
                if (CurrentBlock.Value.LeftBlock.Type == PLCBlockType.SourceBlock && CurrentBlock.Value.RightBlock.Type == PLCBlockType.SourceBlock)
                {
                    // Special case - copying source blocks
                    S7Source rightBlock = null;
                    S7Source leftBlock = null;
                    rightBlock = FindCurrentSourceInProgram(_RightProject, ref _dicRightSimaticCache);
                    leftBlock = FindCurrentSourceInProgram(_LeftProject, ref _dicLeftSimaticCache);

                    //// TODO: wrap it into function
                    //// Find left and right blocks at the same time                
                    //Thread thRight = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Copying Source. Searching for the right block in Simatic structure.");
                    //    rightBlock = FindCurrentSourceInProgram(_RightProject.Programs, ref _dicRightSimaticCache);
                    //    EventFire.Info("Copying Source. Right block found.");
                    //}));
                    //thRight.Start();

                    //Thread thLeft = new Thread(new ThreadStart(() =>
                    // {
                    //    EventFire.Info("Copying Source. Searching for the left block in Simatic structure.");
                    //    leftBlock = FindCurrentSourceInProgram(_LeftProject.Programs, ref _dicLeftSimaticCache);
                    //    EventFire.Info("Copying Source. Left block found.");
                    //}));
                    //thLeft.Start();

                    //// Wait for both threads to complete
                    //thRight.Join();
                    //thLeft.Join();
                    //// TODO: end wrap it into function

                    ProgressBarCurrent++;

                    // Find right block folder
                    IS7Container rightFolder = FindBlockFolderInProgram(CurrentBlock, _RightProject, ref _dicRightSimaticCache);

                    EventFire.Info("Copying. Search complete timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                    S7_BlockCopyMerge mrg = new S7_BlockCopyMerge();
                    if (mrg.CopyBlock(leftBlock, rightBlock, rightFolder))
                    {
                        // Need to refresh right cache
                        EventFire.Info("Copying. Clearing right project cache.");
                        this._dicRightSimaticCache.dicProgramsLogPath.Clear();
                    }
                }
                else
                {
                    // Standard block FC/FB/DB
                    S7Block leftBlock = null;
                    S7Block rightBlock = null;
                    rightBlock = FindCurrentBlockInProgram(_RightProject, ref _dicRightSimaticCache);
                    leftBlock = FindCurrentBlockInProgram(_LeftProject, ref _dicLeftSimaticCache);

                    // TODO: wrap it into function
                    // Find left and right blocks at the same time      
                    //Thread thRight = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Copying. Searching for the right block in Simatic structure.");
                    //    rightBlock = FindCurrentBlockInProgram(_RightProject.Programs, ref _dicRightSimaticCache);
                    //    EventFire.Info("Copying. Right block found.");
                    //}));
                    //thRight.SetApartmentState(ApartmentState.STA);
                    //thRight.Start();

                    //Thread thLeft = new Thread(new ThreadStart(() =>
                    //{
                    //    EventFire.Info("Copying. Searching for the left block in Simatic structure.");
                    //    leftBlock = FindCurrentBlockInProgram(_LeftProject.Programs, ref _dicLeftSimaticCache);
                    //    EventFire.Info("Copying. Left block found.");
                    //}));
                    //thLeft.SetApartmentState(ApartmentState.STA);
                    //thLeft.Start();

                    //// Wait for both threads to complete                                        
                    //thRight.Join();
                    //thLeft.Join();
                    //// TODO: end wrap it into function            

                    ProgressBarCurrent++;

                    // Find right block folder
                    IS7Container rightFolder = FindBlockFolderInProgram(CurrentBlock, _RightProject, ref _dicRightSimaticCache);

                    EventFire.Info("Copying. Search complete timestamp: " + (DateTime.Now - tsSearchTimeStart).TotalSeconds.ToString("0.0") + "s");

                    // Copy block to the right
                    S7_BlockCopyMerge mrg = new S7_BlockCopyMerge();
                    if (mrg.CopyBlock(leftBlock, rightBlock, rightFolder))
                    {
                        // Need to refresh right cache
                        EventFire.Info("Copying. Clearing right project cache.");
                        this._dicRightSimaticCache.dicProgramsLogPath.Clear();
                    }

                    ProgressBarCurrent++;
                }

                ProgressBarCurrent++;

                EventFire.Info("Copying. Refreshing projects.");

                GetBlocksAsync(ExtractRightProjectPath, ExtractLeftProjectPath);

                ProgressBarCurrent = ProgressBarMax;
                EventFire.Info("Copying. Process completed.");
                IsBusy = false;
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        #endregion

        
        #region Helper Functions


        private bool DeleteFolder(string strPath)
        {
            try
            {
                if (Directory.Exists(strPath)) Directory.Delete(strPath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [STAThread]
        private S7Block FindCurrentBlockInProgram(S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                return FindBlockInProgram(CurrentBlock, ProjectS7, ref SimaticCache);
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
            return null;
        }


        [STAThread]
        private S7Block FindBlockInProgram(KeyValuePair<String, Blocks> BlockToFind, S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                List<String> PathParts = (BlockToFind.Key.Split('\\')).ToList<String>();

                // Trying fast method of findig the block
                // Verify that project name is matches
                if (ProjectS7.Name == PathParts[0])
                {
                    // So far so good project seems to be a right one 
                    // Get a station
                    S7Station station = (S7Station)ProjectS7.Stations[PathParts[1]];
                    if (station != null)
                    {
                        // Got a correct station
                        // There is an issue finding a rack by name, so for now assuming it's rack 0
                        S7Rack rack = (S7Rack)station.Racks[0];
                        if (rack != null)
                        {
                            // Got a rack
                            // For unknown reasons we can't search module by name, so for now assuming that CPU is module #2
                            S7Module module = (S7Module)rack.Modules[2];
                            if (module != null && module.Name == PathParts[2])
                            {
                                // Got a CPU module. Now we can find a corresponding program folder
                                S7Programs programs = (S7Programs)ProjectS7.Programs;
                                S7Program program = (S7Program)programs[module];
                                if (program != null && program.Name == PathParts[3])
                                {
                                    // It should be a block subfolder in the program folder
                                    S7SWItem blocksFolder = (S7SWItem)program.Next[PathParts[4]];
                                    if (blocksFolder != null)
                                    {
                                        // Block folder is fine, return a block
                                        return (S7Block)blocksFolder.Next[PathParts.Last()];
                                    }
                                }
                            }
                        }
                    }
                }

                // Fall back to the old method                
                String BlocksFolderPath = BlockToFind.Key.Remove(BlockToFind.Key.LastIndexOf('\\'));
                String ProgramFolderPath = BlocksFolderPath.Remove(BlocksFolderPath.LastIndexOf('\\'));
                SimaticCacheUpdate(ProjectS7, ref SimaticCache);

                var prgs = from ds in SimaticCache.dicProgramsLogPath where ds.Value.strProgramLogPath == ProgramFolderPath select ds;
                if (prgs.Count() > 0)
                {
                    var prg = prgs.First();
                    var blks = from ds in prg.Value.dicBlocksLogPath where ds.Value == BlocksFolderPath select ds;
                    if (blks.Count() > 0)
                    {
                        var blk = blks.First();
                        try
                        {
                            return (S7Block)ProjectS7.Programs[prg.Key].Next[blk.Key].Next[PathParts.Last()];
                        }
                        catch (Exception ex)
                        {
                            EventFire.Error("S7.FindBlockInProgram. Exception: " + ex.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            // Can't find it
            return null;
        }

        [STAThread]
        private IS7Container FindBlockFolderInProgram(KeyValuePair<String, Blocks> FolderToFind, S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                List<String> PathParts = (FolderToFind.Key.Split('\\')).ToList<String>();

                // Trying fast method of findig the block
                S7Program program = FindProgramFolderFast(FolderToFind, ProjectS7);
                if (program != null && program.Name == PathParts[3])
                {
                    // It should be a block subfolder in the program folder
                    return (IS7Container)program.Next[PathParts[4]];
                }

                // Fall back to the old method
                String BlocksFolderPath = FolderToFind.Key.Remove(FolderToFind.Key.LastIndexOf('\\'));
                String ProgramFolderPath = BlocksFolderPath.Remove(BlocksFolderPath.LastIndexOf('\\'));
                SimaticCacheUpdate(ProjectS7, ref SimaticCache);

                var prgs = from ds in SimaticCache.dicProgramsLogPath where ds.Value.strProgramLogPath == ProgramFolderPath select ds;
                if (prgs.Count() > 0)
                {
                    var prg = prgs.First();
                    var blks = from ds in prg.Value.dicBlocksLogPath where ds.Value == BlocksFolderPath select ds;
                    if (blks.Count() > 0)
                    {
                        var blk = blks.First();
                        try
                        {
                            return (IS7Container)ProjectS7.Programs[prg.Key].Next[blk.Key];
                        }
                        catch (Exception ex)
                        {
                            EventFire.Error("S7.FindBlockFolderInProgram. Exception: " + ex.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            // Can't find it
            return null;
        }

        [STAThread]
        private S7Source FindCurrentSourceInProgram(S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                return FindSourceInProgram(CurrentBlock, ProjectS7, ref SimaticCache);
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
            return null;
        }

        [STAThread]
        private S7Source FindSourceInProgram(KeyValuePair<String, Blocks> BlockToFind, S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                List<String> PathParts = (BlockToFind.Key.Split('\\')).ToList<String>();

                // Trying fast method of findig the block
                S7Program program = FindProgramFolderFast(BlockToFind, ProjectS7);
                if (program != null && program.Name == PathParts[3])
                {
                    // It should be a block subfolder in the program folder
                    S7SWItem blocksFolder = (S7SWItem)program.Next[PathParts[4]];
                    if (blocksFolder != null)
                    {
                        // Block folder is fine, return a block
                        return (S7Source)blocksFolder.Next[PathParts.Last()];
                    }
                }

                // Fall back to the old method
                String BlocksFolderPath = BlockToFind.Key.Remove(BlockToFind.Key.LastIndexOf('\\'));
                String ProgramFolderPath = BlocksFolderPath.Remove(BlocksFolderPath.LastIndexOf('\\'));
                SimaticCacheUpdate(ProjectS7, ref SimaticCache);

                var prgs = from ds in SimaticCache.dicProgramsLogPath where ds.Value.strProgramLogPath == ProgramFolderPath select ds;
                if (prgs.Count() > 0)
                {
                    var prg = prgs.First();
                    var blks = from ds in prg.Value.dicBlocksLogPath where ds.Value == BlocksFolderPath select ds;
                    if (blks.Count() > 0)
                    {
                        var blk = blks.First();
                        try
                        {
                            return (S7Source)ProjectS7.Programs[prg.Key].Next[blk.Key].Next[PathParts.Last()];
                        }
                        catch (Exception ex)
                        {
                            EventFire.Error("S7.FindSourceInProgram. Exception: " + ex.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            // Can't find it
            return null;
        }
        

        /// <summary>
        /// FAst method of finding Program Folder. Could potentiaaly fail, so we have to revert back top the old method
        /// </summary>
        /// <param name="ProgramFolderToFind"></param>
        /// <param name="ProjectS7"></param>
        /// <returns></returns>
        [STAThread]
        private S7Program FindProgramFolderFast(KeyValuePair<String, Blocks> ProgramFolderToFind, S7Project ProjectS7)
        {
            try
            {
                List<String> PathParts = (ProgramFolderToFind.Key.Split('\\')).ToList<String>();

                // Trying fast method of findig the block
                // Verify that project name is matches
                if (ProjectS7.Name == PathParts[0])
                {
                    // So far so good project seems to be a right one 
                    // Get a station
                    S7Station station = (S7Station)ProjectS7.Stations[PathParts[1]];
                    if (station != null)
                    {
                        // Got a correct station
                        // There is an issue finding a rack by name, so for now assuming it's rack 0
                        S7Rack rack = (S7Rack)station.Racks[0];
                        if (rack != null)
                        {
                            // Got a rack
                            // For unknown reasons we can't search module by name, so for now assuming that CPU is module #2
                            S7Module module = (S7Module)rack.Modules[2];
                            if (module != null && module.Name == PathParts[2])
                            {
                                // Got a CPU module. Now we can find a corresponding program folder
                                S7Programs programs = (S7Programs)ProjectS7.Programs;
                                return (S7Program)programs[module];
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }

            // Can't find it
            return null;
        }


        [STAThread]
        private void SimaticCacheUpdate(S7Project ProjectS7, ref SimaticCache SimaticCache)
        {
            try
            {
                // if dictionary is not populated
                if (SimaticCache.dicProgramsLogPath.Count == 0)
                {
                    for (int i = 1; i <= ProjectS7.Programs.Count; i++)
                    {
                        if (ProjectS7.Programs[i].Type == S7ProgramType.S7)
                        {
                            SimaticCache.dicProgramsLogPath.Add(i, new SimaticCache.SimaticBlocksCache());

                            //Debug.Print("SimaticCacheUpdate. Program: "+ i.ToString() + "; Before LogPath: " + DateTime.Now.ToLocalTime().ToString());                    
                            SimaticCache.dicProgramsLogPath[i].strProgramLogPath = ProjectS7.Programs[i].LogPath;
                            //Debug.Print("SimaticCacheUpdate. Program: " + i.ToString() + "; After LogPath: " + DateTime.Now.ToLocalTime().ToString());

                            for (int j = 1; j <= ProjectS7.Programs[i].Next.Count; j++)
                            {
                                SimaticCache.dicProgramsLogPath[i].dicBlocksLogPath.Add(j, ProjectS7.Programs[i].Next[j].LogPath);
                                Console.WriteLine("Adding [" + j + "]  " + ProjectS7.Programs[i].Next[j].LogPath);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        [STAThread]
        private void LoadingBothSimaticProjects (ref Simatic _Simatic)
        {
            
            try
            {
                // Trying to find loaded projects
                EventFire.Info("Loading left project to Simatic.");
                if (ExtractLeftProjectPath != "")
                {
                    _LeftProject = getSimaticProject(ref _Simatic, GetProjectFileNameFromPath(ExtractLeftProjectPath));
                }
                else
                {
                    if (IsZippedProject(LeftProjectPath))
                    {
                        // Zipped project - Unzip it first
                        ExtractLeftProjectPath = Path.GetTempPath() + "DMC_Merge" + "\\pLeft\\";
                        _LeftProject = getSimaticProject(ref _Simatic, UnzipProject(LeftProjectPath, ExtractLeftProjectPath));
                    }
                    else
                    {
                        // Unzipped project - attach
                        ExtractLeftProjectPath = LeftProjectPath;
                        _LeftProject = getSimaticProject(ref _Simatic, GetProjectFileNameFromPath(LeftProjectPath));
                    }
                }
                ProgressBarCurrent++;

                EventFire.Info("Loading right project to Simatic.");
                if (ExtractRightProjectPath != "")
                {
                    _RightProject = getSimaticProject(ref _Simatic, GetProjectFileNameFromPath(ExtractRightProjectPath));
                }
                else
                {
                    if (IsZippedProject(RightProjectPath))
                    {
                        // Zipped project - Unzip it first
                        ExtractRightProjectPath = Path.GetTempPath() + "DMC_Merge" + "\\pRight\\";
                        _RightProject = getSimaticProject(ref _Simatic, UnzipProject(RightProjectPath, ExtractRightProjectPath));
                    }
                    else
                    {
                        // Unzipped project - attach
                        ExtractRightProjectPath = RightProjectPath;
                        _RightProject = getSimaticProject(ref _Simatic, GetProjectFileNameFromPath(RightProjectPath));
                    }
                }
                ProgressBarCurrent++;
            }
            catch (Exception err)
            {
                EventFire.Error(err.ToString());
            }
        }

        [STAThread]
        private S7Project getSimaticProject(ref Simatic _Simatic, string ProjectPath)
        {
            if (ProjectPath.ToLower().EndsWith(".s7p"))
            {
                return (S7Project)_Simatic.Projects.Add(ProjectPath);
            }
            else
            {
                var Projects = Directory.EnumerateFiles(ProjectPath, "*.s7p", SearchOption.AllDirectories);
                if (Projects.Count() > 0)
                {
                    return (S7Project)_Simatic.Projects.Add(Projects.First());
                }
                else
                {
                    throw new FileNotFoundException("s7p file not found in the folder!");
                }
            }
        }


        private bool IsZippedProject(string ProjectPath)
        {
            return ProjectPath.ToLower().EndsWith(".zip");
        }


        private string UnzipProject(string ProjectPath, string ExtractedProjectPath)
        {
            if (ProjectPath.ToLower().EndsWith(".zip"))
            {
                // Clear extracted folder
                DeleteFolder(ExtractedProjectPath);

                try
                {
                ZipHelper zip = new ZipHelper("");

                zip.UnpackToFolder(ProjectPath, ExtractedProjectPath);
                var Projects = Directory.EnumerateFiles(ExtractedProjectPath, "*.s7p", SearchOption.AllDirectories);
                if (Projects.Count() > 0)
                {
                    // Return path to the s7p file
                    return Projects.First();
                }
                }
                catch (Exception ex)
                {
                    EventFire.Error("S7.UnzipProject. Exception: " + ex.ToString());
                    return "";
                }
            }

            return "";
        }


        private void saveZippedProject(string ExtractedProjectPath, string ProjectPath)
        {
            if (ProjectPath.ToLower().EndsWith(".s7p"))
            {
                // Unpacked project - do nothing
            }
            else if (!ProjectPath.ToLower().EndsWith(".zip"))
                throw new ArgumentException("Path must be .s7p or .zip!", "Path");
            else 
            {
                try
                {
                    //if (_Simatic != null)
                    //{
                    //    _Simatic.Close();
                    //    _Simatic = null;
                    //}

                    // Archive with SharpZipLib, it seems to be faster
                    File.Delete(ProjectPath);
                    ZipHelper zip = new ZipHelper("");
                    zip.ArchiveFolder(ExtractedProjectPath, ProjectPath);

                    ExtractRightProjectPath = "";
                    ExtractLeftProjectPath = "";
                }
                catch (Exception ex)
                {
                    EventFire.Error("S7.saveZippedProject. Exception: " + ex.ToString());
                }
            }
        }

        
        #endregion

        #region Multi-threaded Helpers
        private Thread jobThread;
        private Thread workerThread;


        private void DoJob(ThreadStart workerThreadStart)
        {
            jobThread = new Thread(JobThread);
            jobThread.SetApartmentState(ApartmentState.STA);
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

            IsBusy = true;

            workerThread = new Thread(workerThreadStart as ThreadStart);
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
            
  
            // Wait for worker thread to finish
            workerThread.Join();
            IsBusy = false;
           
        }

        #endregion


    }
}
 