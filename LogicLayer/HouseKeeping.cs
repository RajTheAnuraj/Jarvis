using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    public class HouseKeeping
    {
        
        public static string RootFolder { get; set; }

        public static string Slash = "\\";
        

        public static string GetSystemFolder()
        {
            string AppendedPath = HouseKeeping.AppenedFileSystemPath(RootFolder,"System");
            return AppendedPath;
        }

        public static string AppenedFileSystemPath(params string[] p)
        {
            return String.Join(Slash, p);
        }

        public static void CreateDirectory(string FolderPath)
        {
            if (Directory.Exists(FolderPath))
                return;
            string restOfThePath = FolderPath.Replace(RootFolder,"");
            string[] restOfThePathParts = restOfThePath.Split(new string[] { Slash }, StringSplitOptions.RemoveEmptyEntries);

            restOfThePath = RootFolder;
            foreach (string parts in restOfThePathParts)
            {
                restOfThePath = restOfThePath + Slash + parts;
                if (Directory.Exists(restOfThePath)) continue;

                Directory.CreateDirectory(restOfThePath);
            }
        }

    }
}
