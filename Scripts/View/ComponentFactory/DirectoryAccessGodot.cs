using CAP_Contracts;
using CAP_DataAccess.Helpers;
using Godot;
using System;

namespace ConnectAPIC.Scripts.View.ComponentFactory
{
    public class DirectoryAccessGodot : IDirectoryAccess
    {
        public DirectoryAccessGodot()
        {
        }
        public DirectoryAccessGodot(string path) :this()
        {
            this.dirAccess = DirAccess.Open(path);
        }
        private DirAccess dirAccess;

        public void Dispose()
        {
            dirAccess.Dispose();
        }

        public string GetNext()
        {
            return dirAccess.GetNext();
        }

        public bool CurrentIsDir()
        {
            return dirAccess.CurrentIsDir();
        }

        public IDirectoryAccess Open(string path)
        {
            var newDirObj = new DirectoryAccessGodot(path);
            newDirObj.dirAccess = DirAccess.Open(path);
            
            if (newDirObj.dirAccess != null)
            {
                newDirObj.dirAccess.ListDirBegin();
            }
            return newDirObj;
        }
    }
}
