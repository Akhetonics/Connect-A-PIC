using System;

namespace CAP_Contracts
{
    public interface IDirectoryAccess : IDisposable
    {
        IDirectoryAccess Open(string path);
        bool CurrentIsDir(); // returns true if it is a directory, or false if there is no next element or if it is a file
        string GetNext(); // gets next Name of Dir or File or "" if there is no next element. 

    }
}