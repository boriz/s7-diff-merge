using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DMCBase
{
    public static class CommonFunctions
    {
        public static void CopyEntireDirectory(String SourcePath, String DestinationPath, bool OverwriteExisting)
        {
            // First create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            // Copy all the files
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), OverwriteExisting);
                new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Read, new string[] { newPath.Replace(SourcePath, DestinationPath) }).Demand();
            }
        }
    }
}
