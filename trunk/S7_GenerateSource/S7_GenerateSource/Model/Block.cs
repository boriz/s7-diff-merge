using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimaticLib;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace S7_GenerateSource
{
    partial class Block
    {
        public int? Size { get; set; }
        public String Name { get; set; }
        public PLCBlockType Type { get; set; }
        public String TypeString { get; set; }
        public PLCLanguage Language { get; set; }
        public String LanguageString { get; set; }
        public String SymbolicName { get; set; }
        public DateTime Modified { get; set; }  
      
        public BlockSimilarityType SizeSimilarity {get;set;}
        public BlockSimilarityType NameSimilarity { get; set; }
        public BlockSimilarityType SymbolicNameSimilarity { get; set; }
        public BlockSimilarityType ModifiedSimilarity { get; set; }

        public BlockSimilarityType Similarity 
        { 
            get
            {
                if (SizeSimilarity == BlockSimilarityType.Orphan ||
                    NameSimilarity == BlockSimilarityType.Orphan ||
                    SymbolicNameSimilarity == BlockSimilarityType.Orphan ||
                    ModifiedSimilarity == BlockSimilarityType.Orphan )
                {
                    return BlockSimilarityType.Orphan;
                }

                if (SizeSimilarity == BlockSimilarityType.Identical &&
                    NameSimilarity== BlockSimilarityType.Identical &&
                    SymbolicNameSimilarity== BlockSimilarityType.Identical &&
                    ModifiedSimilarity== BlockSimilarityType.Identical )
                {
                    return BlockSimilarityType.Identical;
                }

                return BlockSimilarityType.Different;
            }
        }

    }

    public enum BlockSimilarityType
    {
        Identical,
        Orphan,
        Different,
    }
}
