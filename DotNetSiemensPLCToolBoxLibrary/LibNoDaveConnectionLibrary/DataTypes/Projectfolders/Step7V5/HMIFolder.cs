using System;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class HMIFolder : Step7ProjectFolder
    {
        public int UnitID { get; set; }
        public HMIType Type { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }


    public enum HMIType
    {
        Unknown = 0,
        WinCCFlexible = 1,
    }
}
