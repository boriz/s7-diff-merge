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


namespace S7_GenerateSource
{
    partial class S7_BlockCopyMerge
    {

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string className, string windowTitle);


    #region Private Variables

        private String _LeftSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sL_";
        private String _RightSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sR_";
        private String _MergedSourceFilePath = Path.GetTempPath() + "DMC_Merge" + "\\sM_";

    #endregion

        [STAThread]
        public void MergeBlocks(S7Block leftBlock, S7Block rightBlock)
        {
            // Standard FB/FC/DB

            // Should have both blocks
            if (leftBlock == null || rightBlock == null)
            {
                EventFire.Error("Merge S7 Blocks. Can't find block in the Simatic structure.");
                return;
            }

            EventFire.Info("Merge S7 Blocks. Generating right source.");
            if (!Directory.Exists(_RightSourceFilePath)) Directory.CreateDirectory(_RightSourceFilePath);
            String rightSourceFileName = _RightSourceFilePath + rightBlock.Name + ".awl";
            rightBlock.GenerateSource(rightSourceFileName, S7GenerateSourceFlags.S7GSFDoOverwrite);

            EventFire.Info("Merge S7 Blocks. Generating left source.");
            if (!Directory.Exists(_LeftSourceFilePath)) Directory.CreateDirectory(_LeftSourceFilePath);
            String leftSourceFileName = _LeftSourceFilePath + leftBlock.Name + ".awl";
            leftBlock.GenerateSource(leftSourceFileName, S7GenerateSourceFlags.S7GSFDoOverwrite);

            String mergedSourceFileName = _MergedSourceFilePath + rightBlock.Name + ".awl";

            Process DiffProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "TortoiseMerge.exe",
                    Arguments = "/base:\"" + leftSourceFileName + "\" /mine:\"" + rightSourceFileName + "\" /merged:\"" + mergedSourceFileName + "\""
                }
            };
            DiffProcess.Start();
            DiffProcess.WaitForExit();

            if (File.Exists(mergedSourceFileName))
            {
                S7SWItems s7swItems = rightBlock.Program.Next;
                for (int i = 1; i <= s7swItems.Count; i++)
                {
                    if (((S7Container)s7swItems[i]).ConcreteType == S7ContainerType.S7SourceContainer)
                    {
                        IS7Source3 s7Source = (IS7Source3)s7swItems[i].Next.Add("Tmprc", S7SWObjType.S7Source, mergedSourceFileName);

                        EventFire.Info("Merge S7 Blocks. Compiling block.");
                        S7SWItems s7swReturn = s7Source.Compile();
                        IntPtr hwnd = (IntPtr)s7Source.AppWindowHandle;
                        ShowWindow(hwnd, 6);    // 0 = hide the window, 6 = minimize

                        if (s7swReturn.Count <= 0)
                        {
                            // Something is not right - show edit window
                            EventFire.Error("Merge S7 Blocks. Can't compile block, please resolve compilation errors.");
                            ShowWindow(hwnd, 3);    // 3 - maximize
                            s7Source.Edit();

                            // Wait till they fixed it and close the window
                            while (IsWindow(hwnd)) ;
                        }
                        else
                        {
                            // Close editor window
                            EventFire.Info("Merge S7 Blocks. Compiled successfully.");
                            //SendMessage(hwnd, 0x0112, 0xF060, 0); // 0xF060 = SC_CLOSE; 0x0112 = WM_SYSCOMMAND                                
                        }

                        // Remove the source                            
                        s7Source.Remove();

                        // S7 editor doesn't like close message. Minimize it
                        ShowWindow(hwnd, 6);

                        break;
                    }
                }
            }
        }


        public void MergeBlocks(S7Source leftBlock, S7Source rightBlock, string ExtractRightProjectPath, KeyValuePair<String, Blocks> CurrentBlock)
        {
            // Should have both blocks
            if (leftBlock == null || rightBlock == null)
            {
                EventFire.Error("Merge Source Blocks. Can't find block in the Simatic structure.");
                return;
            }

            EventFire.Info("Merge Source Blocks. Exporting right source.");
            if (!Directory.Exists(_RightSourceFilePath)) Directory.CreateDirectory(_RightSourceFilePath);
            String rightSourceFileName = _RightSourceFilePath + rightBlock.Name + ".scl";
            rightBlock.Export(rightSourceFileName);

            EventFire.Info("Merge Source Blocks. Exporting left source.");
            if (!Directory.Exists(_LeftSourceFilePath)) Directory.CreateDirectory(_LeftSourceFilePath);
            String leftSourceFileName = _LeftSourceFilePath + leftBlock.Name + ".scl";
            leftBlock.Export(leftSourceFileName);

            String mergedSourceFileName = _MergedSourceFilePath + rightBlock.Name + ".scl";

            Process DiffProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "TortoiseMerge.exe",
                    Arguments = "/base:\"" + leftSourceFileName + "\" /mine:\"" + rightSourceFileName + "\" /merged:\"" + mergedSourceFileName + "\""
                }
            };
            DiffProcess.Start();
            DiffProcess.WaitForExit();

            if (File.Exists(mergedSourceFileName))
            {
                // Can't find any gracefull way of importing source block - just override the file
                String strSrcFile = ExtractRightProjectPath + ((BlockSource)CurrentBlock.Value.RightBlock).Filename.Replace(@"/", @"\");
                File.Copy(mergedSourceFileName, strSrcFile, true);

                EventFire.Info("Merge Source Blocks. Compiling block");
                S7SWItems s7swReturn = rightBlock.Compile();

                // AppWindowHandle crashes. Find window hwnd and wait till they done editing/compiling
                //IntPtr hwnd = (IntPtr)rightBlock.AppWindowHandle;
                IntPtr hwnd = IntPtr.Zero;

                Process[] processRunning = Process.GetProcesses();
                Process proc = null;
                foreach (Process pr in processRunning)
                {
                    if (pr.ProcessName == "s7sclapx")
                    {
                        proc = pr;
                        hwnd = pr.Handle;
                        break;
                    }
                }

                //ShowWindow(hwnd, 6);    // 0 = hide the window, 6 = minimize

                // Seems to be a bug, compile always returns 0
                if (s7swReturn.Count <= 0)
                {
                    // Something is not right - show edit window
                    //EventFire.Error("Can't compile block, please resolve compilation errors");
                    EventFire.Info("Merge Source Blocks. Please review/compile block and close SCL editor.");
                    ShowWindow(hwnd, 3);    // 3 - maximize
                    rightBlock.Edit();                    
                    
                    // Wait till they fixed it and close the window
                    //while (IsWindow(hwnd)) ;
                    while (!proc.HasExited) ;
                }
                else
                {
                    // Close editor window
                    EventFire.Info("Merge Source Blocks. Compiled successfully");
                    SendMessage(hwnd, 0x0112, 0xF060, 0); // 0xF060 = SC_CLOSE; 0x0112 = WM_SYSCOMMAND                                
                }
            }
        }


        /// <summary>
        /// Copy S7 block to the right. Returns true if right project structure changed (block added/removed)
        /// </summary>
        /// <param name="leftBlock"></param>
        /// <param name="rightBlock"></param>
        /// <param name="rightFolder"></param>
        /// <returns></returns>
        public bool CopyBlock(S7Block leftBlock, S7Block rightBlock, IS7Container rightFolder)
        {
            // Standard blocks FB/FC/DB

            bool bRes = false;

            if (leftBlock != null)
            {
                // Source block exists
                if (rightBlock != null)
                {
                    // Target block also exists - copy!
                    EventFire.Info("Copying S7 Block. Copying block to the right.");

                    // Magic happened here. Deleting and re-creating block doesn't affect cache
                    // Because new block somehow gets created with the same array index as the old one
                    rightBlock.Remove();
                    leftBlock.Copy(rightBlock.Parent);
                    EventFire.Info("Copying S7 Block. Done.");
                }
                else
                {                
                    if (rightFolder != null)
                    {
                        // Right block does not exists - create a new one
                        EventFire.Info("Copying S7 Block. Right block does not exisits. Creating new block.");
                        leftBlock.Copy(rightFolder);
                        EventFire.Info("Copying S7 Block. Done.");

                        // Just created a new block - need to update right cache
                        // TODO: Smart cache update - only current folder
                        bRes = true;
                    }
                    else
                    {
                        EventFire.Error("Copying S7 Block. Right block does not exisits. Can't find corresponding parent foder in the right Simatic project.");
                    }
                }
            }
            else
            {
                // Source block does not exists - remove target block
                EventFire.Info("Copying S7 Block. Left block does not exists. Deleting right block.");
                rightBlock.Remove();

                // TODO: Maybe we don't need to refresh cache? Verify
                // Just deleted a block - need to update cache
                bRes = true;
                EventFire.Info("Copying S7 Block. Done.");
            }

            return bRes;
        }

   
        /// <summary>
        /// Copy source block to the right
        /// </summary>
        /// <param name="CurrentBlock"></param>
        public bool CopyBlock(S7Source leftBlock, S7Source rightBlock, IS7Container rightFolder)
        {
            // Copying source block

            bool bRes = false;

            if (leftBlock != null)
            {
                // Source block exists
                if (rightBlock != null)
                {
                    // Target block also exists - copy!
                    EventFire.Info("Copying Source Block. Copying block to the right.");
                    
                    // Magic happened here. Deleting and re-creating block doesn't affect cache
                    // Because new block somehow gets created with the same array index as the old one
                    rightBlock.Remove();
                    leftBlock.Copy(rightBlock.Parent);                    
                }
                else
                {
                    if (rightFolder != null)
                    {
                        // Right block does not exists - create a new one
                        EventFire.Info("Copying Source Block. Right block does not exisits. Creating new block.");
                        leftBlock.Copy(rightFolder);

                        // Just created a new block - need to update right cache
                        // TODO: Smart cache update - only current folder
                        bRes = true;
                    }
                    else
                    {
                        EventFire.Error("Copying Source Block. Right block does not exisits. Can't find corresponding parent foder in the right Simatic project.");
                        return false;
                    }
                }

                // TODO: Enable compiling block after copy
                //// Source copied, time to compile it
                //EventFire.Info("Copying Source Blocks. Compiling block");
                //S7SWItems s7swReturn = rightBlock.Compile();

                //// AppWindowHandle crashes. Find window hwnd and wait till they done editing/compiling
                //IntPtr hwnd = IntPtr.Zero;

                //Process[] processRunning = Process.GetProcesses();
                //Process proc = null;
                //foreach (Process pr in processRunning)
                //{
                //    if (pr.ProcessName == "s7sclapx")
                //    {
                //        proc = pr;
                //        hwnd = pr.Handle;
                //        break;
                //    }
                //}

                //// TODO: Seems to be a bug, Compile always returns 0
                //EventFire.Info("Copying Source Block. Please review/compile block and close SCL editor.");
                //ShowWindow(hwnd, 3);    // 3 - maximize
                //rightBlock.Edit();

                //// Wait till they fixed it and close the window
                //while (!proc.HasExited);

                EventFire.Info("Copying Source Block. Done.");
            }
            else
            {
                // Source block does not exists - remove target block
                EventFire.Info("Copying Source Block. Left block does not exists. Deleting right block.");
                rightBlock.Remove();

                EventFire.Info("Copying Source Blocks. Done.");

                // Just deleted a block - need to update cache    
                // TODO: Smart cache update - only current folder
                bRes = true;
            }

            return bRes;
        }

    }
}
