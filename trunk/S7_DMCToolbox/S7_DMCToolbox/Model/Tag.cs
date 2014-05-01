using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using Trending;


namespace S7_DMCToolbox
{
     public class Tag
    {

        public String Name { get; set; }
        public AREA_TYPE AreaTypeParameter { get; set; }
        public DATA_TYPES DataType {get;set;}
        public int DbNumber { get; set; }
        public int ByteOffset { get; set; }
        public int BitOffset { get; set; }
        public String Address { get; set; }
        public bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        
    }
    
}
