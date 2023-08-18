using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Helpers
{
    public class FileSaver
    {
        public static void SaveToFile(string content, string filePath)
        {
            if (!IsValidFilePath(filePath))
            {
                return;
            }
            File.WriteAllText(filePath, content);
        }

        private static bool IsValidFilePath(string filePath)
        {
            try
            {
                Path.GetFullPath(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
