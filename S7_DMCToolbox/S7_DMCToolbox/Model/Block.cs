using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;


namespace S7_DMCToolbox
{
    partial class Block
    {
        public int Size { get; set; }
        public String Name { get; set; }
        public String SymbolicName { get; set; }
        public DateTime Modified { get; set; }
        public BlockSimilarityType SizeSimilarity {get;set;}
        public BlockSimilarityType NameSimilarity { get; set; }
        public BlockSimilarityType SymbolicNameSimilarity { get; set; }
        public BlockSimilarityType ModifiedSimilarity { get; set; }
        public S7Block BlockContents { get; set; }
    }
    public enum BlockSimilarityType
    {
        Identical,
        Orphan,
        Different,
    }
}
