using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SourceFolder : Step7ProjectFolder,IBlocksFolder
    {
        public string Folder { get; set; }

        public List<ProjectBlockInfo> readPlcBlocksList()
        {
            bool showDeleted = ((Step7ProjectV5)this.Project)._showDeleted;

            List<ProjectBlockInfo> tmpBlocks = new List<ProjectBlockInfo>();

            if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "S7CONTAI.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "S7CONTAI.DBF", ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                foreach (DataRow row in dbfTbl.Rows)
                {
                    if (!(bool)row["DELETED_FLAG"] || showDeleted)
                    {

                        S7ProjectSourceInfo tmp = new S7ProjectSourceInfo();
                        tmp.Deleted = (bool)row["DELETED_FLAG"];
                        tmp.Name = (string)row["NAME"];
                        tmp.Filename = Folder + (string)row["FILENAME"];
                        tmp.ParentFolder = this;                        
                        tmp.id = (int)row["ID"];
                        tmp.BlockType = PLCBlockType.SourceBlock;

                        //tmp.Cre
                        tmpBlocks.Add(tmp);
                    }
                }
            }
            return tmpBlocks;
        }

        public List<ProjectBlockInfo> BlockInfos
        {
            get { return readPlcBlocksList(); }
        }

        public ProjectBlockInfo GetProjectBlockInfoFromBlockName(string BlockName)
        {
            var tmp = readPlcBlocksList();
            foreach (ProjectPlcBlockInfo step7ProjectBlockInfo in tmp)
            {
                if (step7ProjectBlockInfo.BlockType.ToString() + step7ProjectBlockInfo.BlockNumber.ToString() == BlockName.ToUpper())
                    return step7ProjectBlockInfo;
            }
            return null;
        }

        public Block GetBlock(string BlockName)
        {
            var prjBlkInf = GetProjectBlockInfoFromBlockName(BlockName);
            if (prjBlkInf != null)
                return GetBlock(prjBlkInf);
            return null;
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            S7ProjectSourceInfo srcInfo = (S7ProjectSourceInfo)blkInfo;

            S7SourceBlock retVal = new S7SourceBlock();

            retVal.Name = srcInfo.Name;
            retVal.BlockNumber = blkInfo.id;
            retVal.ParentFolder = srcInfo.ParentFolder;
            retVal.Filename = srcInfo.Filename;
            retVal.BlockType = srcInfo.BlockType;
            retVal.BlockLanguage = PLCLanguage.SRC;
            

            // Read header
            if (((Step7ProjectV5)Project)._ziphelper.FileExists(Folder + "S7CONTAI.DBF"))
            {
                var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "S7CONTAI.DBF", ((Step7ProjectV5)Project)._ziphelper, ((Step7ProjectV5)Project)._DirSeperator);
                DataRow[] bstRows = dbfTbl.Select("ID = " + blkInfo.id);

                // Should get only one record
                if (bstRows.Length == 1)
                {
                    DataRow bstRow = bstRows[0];
                    retVal.Author = (string)bstRow["CREATOR"];             
                    retVal.Description = (string)bstRow["COMMENT"];
                    // TODO: Parse CRDATE1/2 fields in the DBF
                    int ver = ((int)bstRow["VERSION"]);
                    retVal.Version = (ver / 15).ToString() + "." + (ver % 15).ToString();
                }
            }


            // Read source file
            if (((Step7ProjectV5)Project)._ziphelper.FileExists(srcInfo.Filename))
            {
                // Get a file timestamp for now
                retVal.LastCodeChange = ((Step7ProjectV5)Project)._ziphelper.FileModDateTime(srcInfo.Filename);
                Stream strm = ((Step7ProjectV5)Project)._ziphelper.GetReadStream(srcInfo.Filename);

                retVal.Text = new System.IO.StreamReader(strm,Encoding.UTF7).ReadToEnd();
                //ReadToEnd();
            }

            // Block size
            retVal.CodeSize = retVal.Text.Length;

            return retVal;
        }
    }
}
