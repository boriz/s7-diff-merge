using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7SourceBlock:S7Block
    {
        public string Text { get; set; }

        public string Filename { get; set; }

        private string _description;
        public string Description { get; set; }


        public override string BlockName 
        { 
            get 
            {
                return this.Name;
            } 
        }


        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();

            retVal.Append(BlockType.ToString());
            retVal.Append(BlockNumber.ToString());
            retVal.Append(" : ");
            if (Name != null)
                retVal.Append(Name);
            retVal.Append("\r\n\r\n");

            if (Description != null)
            {
                retVal.Append("Description\r\n\t");
                retVal.Append(Description.Replace("\n", "\r\n\t"));
                retVal.Append("\r\n\r\n");
            }

            retVal.Append("SRC-Code\r\n");

            retVal.Append(Text);
            retVal.Append("\r\n");

            return retVal.ToString();
        }
    }
}
