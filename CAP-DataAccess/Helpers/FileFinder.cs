using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_DataAccess.Helpers
{
    public class FileFinder
    {
        public FileFinder(IDirectoryAccess directoryAccess)
        {
            DirectoryAccess = directoryAccess;
        }

        public IDirectoryAccess DirectoryAccess { get; }

        public List<string> FindRecursively(string path, string extensionPattern)
        {
            var paths = new List<string>();

            using var dir = DirectoryAccess.Open(path);
            if (dir != null)
            {
                string fileName = dir.GetNext();
                while (fileName != "")
                {
                    if (dir.CurrentIsDir())
                    {
                        paths.AddRange(FindRecursively(path + "/" + fileName, extensionPattern));
                    }
                    else
                    {
                        if (fileName.EndsWith(extensionPattern, StringComparison.OrdinalIgnoreCase))
                        {
                            paths.Add(path + "/" + fileName);
                        }
                    }
                    fileName = dir.GetNext();
                }
            }
            return paths;
        }
    }
}
