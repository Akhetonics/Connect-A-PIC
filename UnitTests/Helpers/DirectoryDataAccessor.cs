using CAP_Contracts;
using CAP_DataAccess.Helpers;
using CAP_DataAccess.Components.ComponentDraftMapper;

namespace UnitTests
{
    public partial class ComponentDraftFileReaderTests
    {
        public class DirectoryDataAccessor : IDirectoryAccess
        {
            private string[] _entries;
            private int _currentIndex;
            private bool _isCurrentDir;

            public DirectoryDataAccessor() { }
            public DirectoryDataAccessor(string path)
            {
                _entries = Directory.GetFileSystemEntries(path);
                _currentIndex = -1;
            }

            public bool CurrentIsDir()
            {
                if (_currentIndex >= 0 && _currentIndex < _entries.Length)
                {
                    return _isCurrentDir;
                }
                return false;
            }

            public void Dispose()
            {
            }

            public string GetNext()
            {
                _currentIndex++;
                if (_currentIndex < _entries.Length)
                {
                    _isCurrentDir = Directory.Exists(_entries[_currentIndex]);
                    var next = Path.GetFileName(_entries[_currentIndex]);
                    return next;
                }
                return "";
            }

            public IDirectoryAccess Open(string path)
            {
                if (Directory.Exists(path))
                {
                    return new DirectoryDataAccessor(path);
                }
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
        }
    }
}
