using CAP_Contracts;
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

        /// <summary>
        /// returns a list of paths of all found files with the given extension pck for example or json
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensionPattern">The extension pattern should only be the extension without * or a dot</param>
        /// <returns></returns>
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
