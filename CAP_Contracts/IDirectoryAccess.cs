using System;

namespace CAP_DataAccess.Helpers
{
    public interface IDirectoryAccess : IDisposable
    {
        IDirectoryAccess Open(string path);
        bool CurrentIsDir();
        string GetNext();

    }
}