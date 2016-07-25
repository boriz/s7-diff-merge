using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Ookii.Dialogs.Wpf;

namespace DotNetSiemensPLCToolBoxLibrary.Source
{
    public static class UdtSourceParser
    {
        #region Public Properties

        public static Dictionary<String, S7DataBlock> UdtReferenceBlocks = new Dictionary<String, S7DataBlock>();

        #endregion

        #region Public Methods

        public static Dictionary<String, S7DataBlock> ParseUdtSourceFile(String filePath, int addressOffset = 0)
        {
            List<String> inputFileContents = new List<String>(File.ReadAllLines(filePath));

            return ReadUdtSource(inputFileContents, addressOffset);

        }

        public static Dictionary<String, S7DataBlock> ParseUdtSourceFile(List<String> fileContents, int addressOffset = 0)
        {
            return ReadUdtSource(fileContents, addressOffset);
        }

        public static Dictionary<String, S7DataBlock> ParseUdtSourceFile(String[] fileContents, int addressOffset = 0)
        {
            return ReadUdtSource(new List<String>(fileContents), addressOffset);
        }

        public static S7DataRow ParseChildrenRowsFromText(List<String> inputText, String structName, String structComment, DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Block blockName)
        {
            S7DataRow newRow = new S7DataRow(structName, S7DataRowType.STRUCT, blockName);
            newRow.Comment = structComment;
            bool finished = false;

            while (inputText.Count > 0)
            {
                String line = inputText[0];
                inputText.RemoveAt(0);
                String trimmedLine = line.Trim();
                String comment = "";
                String type = "";
                String name = "";
                String arrayType = "";
                String[] splitString;
                S7DataRow childRow = new S7DataRow("", S7DataRowType.ANY, blockName);
                int length = 0;
                int arrayStart = 0;
                bool typeFailed = false;

                if (trimmedLine.Contains("//"))
                {
                    splitString = trimmedLine.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                    comment = splitString[1].Trim();
                    trimmedLine = splitString[0];
                }
                if (trimmedLine.Contains(":"))
                {
                    splitString = trimmedLine.Split(':');
                    type = splitString[1].Trim().Trim(';').Trim('\"').Trim();
                    name = splitString[0].Trim();

                    if (type.ToUpper().Contains("ARRAY"))
                    {
                        name = name.Trim('\"');
                        splitString = type.Split(new string[] { "[", "..", "]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitString.Length == 4)
                        {
                            bool parsed = Int32.TryParse(splitString[2], out length);
                            bool parsed2 = Int32.TryParse(splitString[1], out arrayStart);
                        }
                        splitString = type.Split(new string[] { "of" }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitString.Length > 1)
                        {
                            arrayType = splitString[1].Trim();
                        }

                        type = "ARRAY";

                    }
                    else if (type.ToUpper().Contains("STRING"))
                    {
                        splitString = type.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitString.Length > 1)
                        {
                            bool parsed = Int32.TryParse(splitString[1], out length);
                        }
                        else
                        {
                            length = 254;
                        }

                        type = "STRING";

                    }

                }

                comment = String.IsNullOrEmpty(comment) ? name : comment;

                if (trimmedLine.ToUpper().Contains("END_STRUCT"))
                {
                    finished = true;
                    break;
                }

                switch (type.ToUpper())
                {
                    case "":
                        typeFailed = true;
                        break;
                    case "ARRAY":
                        S7DataRowType dataType;
                        bool parsed = Enum.TryParse<S7DataRowType>(arrayType.ToUpper(), out dataType);
                        if (!parsed)
                        {
                            typeFailed = GetUdtStructureFromSource(arrayType, out childRow);
                            childRow.Name = name;
                            childRow.DataType = S7DataRowType.UDT;
                        }
                        else
                        {
                            childRow = new S7DataRow(name, dataType, blockName);
                        }

                        childRow.IsArray = true;
                        childRow.ArrayStart = new List<int>(new int[]{arrayStart});
                        childRow.ArrayStop = new List<int>(new int[]{length});
                        break;
                    case "STRING":
                        childRow = new S7DataRow(name, S7DataRowType.STRING, blockName);
                        childRow.StringSize = length;
                        break;
                    case "BOOL":
                        childRow = new S7DataRow(name, S7DataRowType.BOOL, blockName);
                        break;
                    case "INT":
                        childRow = new S7DataRow(name, S7DataRowType.INT, blockName);
                        break;
                    case "REAL":
                        childRow = new S7DataRow(name, S7DataRowType.REAL, blockName);
                        break;
                    case "DATE":
                        childRow = new S7DataRow(name, S7DataRowType.DATE, blockName);
                        break;
                    case "DATE_AND_TIME":
                        childRow = new S7DataRow(name, S7DataRowType.DATE_AND_TIME, blockName);
                        break;
                    case "TIME":
                        childRow = new S7DataRow(name, S7DataRowType.TIME, blockName);
                        break;
                    case "TIME_OF_DAY":
                        childRow = new S7DataRow(name, S7DataRowType.TIME_OF_DAY, blockName);
                        break;
                    case "S5TIME":
                        childRow = new S7DataRow(name, S7DataRowType.S5TIME, blockName);
                        break;
                    case "WORD":
                        childRow = new S7DataRow(name, S7DataRowType.WORD, blockName);
                        break;
                    case "BYTE":
                        childRow = new S7DataRow(name, S7DataRowType.BYTE, blockName);
                        break;
                    case "CHAR":
                        childRow = new S7DataRow(name, S7DataRowType.CHAR, blockName);
                        break;
                    case "DINT":
                        childRow = new S7DataRow(name, S7DataRowType.DINT, blockName);
                        break;
                    case "DWORD":
                        childRow = new S7DataRow(name, S7DataRowType.DWORD, blockName);
                        break;
                    case "STRUCT":
                        childRow = ParseChildrenRowsFromText(inputText, name, comment, blockName);
                        break;
                    default:
                        typeFailed = GetUdtStructureFromSource(type, out childRow);
                        childRow.Name = name;
                        break;
                }
                if (typeFailed) { break; }

                childRow.Comment = comment;
                newRow.Add(childRow);

            }

            if (!finished)
            {
                // for some reason we didn't get to an "END_STRUCT" and there is a struct mismatch
                throw new NotImplementedException();
            }

            return newRow;
        }

        #endregion

        #region Private Methods

        private static Dictionary<String, S7DataBlock> ReadUdtSource(List<String> fileContents, int addressOffset)
        {

            Dictionary<String, S7DataBlock> returnBlocks = new Dictionary<String, S7DataBlock>();
            S7DataBlock currentBlock = new S7DataBlock();
            bool startReading = false;
            List<String> blockContents = new List<String>();
            String[] split;

            foreach (String line in fileContents)
            {
                if (line.ToUpper().Contains("TYPE"))
                {
                    split = line.Split(' ');
                    if (split.Length > 1)
                    {
                        currentBlock.Name = split[1].Trim('\"');
                    }
                }

                if (line.ToUpper().Contains("VERSION"))
                {
                    split = line.Split(':');
                    if (split.Length > 1)
                    {
                        currentBlock.BlockVersion = split[1].Trim();
                    }
                }

                if (startReading)
                {
                    if (line.ToUpper().Contains("END_TYPE"))
                    {
                        blockContents = blockContents.Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
                        currentBlock.StructureFromString = ParseChildrenRowsFromText(blockContents, currentBlock.Name, "", currentBlock);
                        currentBlock.StructureFromString.Comment = currentBlock.Name;

                        if (!returnBlocks.ContainsKey(currentBlock.Name))
                        {
                            returnBlocks.Add(currentBlock.Name, currentBlock);
                        }
                        
                        currentBlock = new S7DataBlock();

                        startReading = false;

                    }
                    else
                    {
                        blockContents.Add(line);
                    }
                }
                else
                {
                    if (line.ToUpper().Contains("STRUCT"))
                    {
                        startReading = true;
                    }
                }
            }

            return returnBlocks;
        }

        private static bool GetUdtStructureFromSource(String udtName, out S7DataRow udtStructure)
        {
            bool completed = false;
            bool typeFailed = false;
            S7Block block = new S7Block();
            udtStructure = new S7DataRow(udtName, S7DataRowType.UDT, block);
            while (!completed)
            {
                if (UdtReferenceBlocks.ContainsKey(udtName))
                {
                    udtStructure = UdtReferenceBlocks[udtName].Structure.DeepCopy();
                    completed = true;
                }
                else
                {
                    bool result = PromptForMissingType(udtName);
                    if (!result)
                    {
                        typeFailed = true;
                        break;
                    }
                }
            }

            return typeFailed;
        }

        private static bool PromptForMissingType(String missingType)
        {
            bool result = false;
            DialogResult r = MessageBox.Show("Cannot find type \"" + missingType + "\".\n\rSelect the source file that contains the type definition.", "Missing Type Found", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (r == DialogResult.OK)
            {
                VistaOpenFileDialog selectFileDialog = new VistaOpenFileDialog();

                selectFileDialog.Title = "Select UDT Source Location";
                selectFileDialog.AddExtension = true;
                selectFileDialog.DefaultExt = ".udt";
                selectFileDialog.Filter = "UDT Source|*.udt";

                if ((bool)selectFileDialog.ShowDialog())
                {
                    var tempDictionary = ParseUdtSourceFile(selectFileDialog.FileName);
                    UdtReferenceBlocks = UdtReferenceBlocks.Concat(tempDictionary.Where(d => !(UdtReferenceBlocks.Keys.Contains(d.Key)))).ToDictionary(k => k.Key, v => v.Value);
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}
