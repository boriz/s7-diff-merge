using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.Source
{
    public static class DbSourceParser
    {
        #region Public Methods

        public static Dictionary<String, S7DataBlock> ParseDBSourceFile(String filePath)
        {
            List<String> inputFileContents = new List<String>(File.ReadAllLines(filePath));

            return ReadDBSource(inputFileContents);

        }

        public static Dictionary<String, S7DataBlock> ParseDBSourceFile(List<String> fileContents)
        {
            return ReadDBSource(fileContents);
        }

        public static Dictionary<String, S7DataBlock> ParseDBSourceFile(String[] fileContents)
        {
            return ReadDBSource(new List<String>(fileContents));
        }

        #endregion

        #region Private Methods

        private static Dictionary<String, S7DataBlock> ReadDBSource(List<String> fileContents)
        {

            Dictionary<String, S7DataBlock> returnBlocks = new Dictionary<String, S7DataBlock>();
            S7DataBlock currentBlock = new S7DataBlock();
            bool startReading = false;
            List<String> blockContents = new List<String>();
            String[] split;

            foreach (String line in fileContents)
            {
                if (line.ToUpper().Contains("DATA_BLOCK"))
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
                    if (line.ToUpper().Contains("BEGIN"))
                    {
                        blockContents = blockContents.Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
                        currentBlock.StructureFromString = UdtSourceParser.ParseChildrenRowsFromText(blockContents, currentBlock.Name, "", currentBlock);

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

        #endregion
    }
}
