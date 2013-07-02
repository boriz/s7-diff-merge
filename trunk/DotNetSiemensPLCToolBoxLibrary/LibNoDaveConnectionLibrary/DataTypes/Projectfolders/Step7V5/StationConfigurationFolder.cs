using System;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class StationConfigurationFolder : Step7ProjectFolder
    {
        internal int UnitID;
        public PLCType StationType { get; set; }

        public DateTime HW_Config_Created { get; set; }
        public DateTime HW_Config_Modified { get; set; }

        public long HW_Config_Size { get; set; }
    }
}
