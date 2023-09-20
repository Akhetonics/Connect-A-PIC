using System.Collections.Generic;
using System.IO;

namespace AtetDataFormats
{
    /// <summary>
    /// Handles filesystem data.
    /// </summary>
    public static class FilesystemData
    {
        /// <summary>
        /// Get a list of all directories and files
        /// in the supplied directory.
        /// </summary>
        /// <side-effects>
        /// Reads from the file system.
        /// </side-effects>   
        /// <returns>
        /// A List of Directories and a List of Files.
        /// </returns>   
        public static (List<string>, List<string>) GetFileSystemEntries(string directory, string fileExtensions = null)
        {
            List<string> directories = new List<string>();
            List<string> files = new List<string>();

            try
            {
                foreach (string fsEntry in Directory.EnumerateFileSystemEntries(directory))
                {
                    if (Directory.Exists(fsEntry))
                    {
                        directories.Add(fsEntry);
                    }
                    else
                    {                
                        string extension = Path.GetExtension(fsEntry);

                        if (fileExtensions == null || fileExtensions.Equals(extension, System.StringComparison.InvariantCultureIgnoreCase))
                        {    
                            files.Add(fsEntry);
                        }
                    }
                }
            }
            catch 
            { 
                return (directories, files);
            }
            
            return (directories, files);
        }

        /// <summary>
        /// Read an entire file.
        /// </summary>
        /// <side-effects>
        /// Reads from a file.
        /// </side-effects> 
        /// <returns>
        /// The entire file as a byte array.
        /// </returns>   
        public static byte[] ReadFileBytes(string filename)
        {
            try 
            {
                return File.ReadAllBytes(filename);
            }
            catch
            {
                return new byte[]{};
            }
        }

        /// <summary>
        /// Read an entire file.
        /// </summary>
        /// <side-effects>
        /// Reads from a file.
        /// </side-effects> 
        /// <returns>
        /// The entire file as a string.
        /// </returns>   
        public static string ReadFileString(string filename)
        {
            try 
            {
                return File.ReadAllText(filename);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Extract the filename from a path.
        /// </summary>
        /// <returns>
        /// The file name.
        /// </returns>   
        public static string GetFileNameWithoutExtension(string path)
        {
            try 
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Extract the directory from a path.
        /// </summary>
        /// <returns>
        /// The directory name.
        /// </returns>   
        public static string GetDirectory(string path)
        {
            try 
            {
                return Path.GetDirectoryName(path);
            }
            catch
            {
                return "";
            }
        }
    }
}
