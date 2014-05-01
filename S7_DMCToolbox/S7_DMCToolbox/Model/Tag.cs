using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;


namespace S7_DMCToolbox
{
    partial class Tag
    {

        public String Name { get; set; }
        public String Type { get; set; }
        public int DbNumber { get; set; }
        public int ByteOffset { get; set; }
        public int BitOffset { get; set; }
        public String Address { get; set; }
    }
    
}
