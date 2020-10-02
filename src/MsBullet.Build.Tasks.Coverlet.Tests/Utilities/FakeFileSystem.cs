// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.IO;
using Coverlet.Core.Abstractions;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    internal class FakeFileSystem : IFileSystem
    {
        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string path)
        {
            throw new NotImplementedException();
        }

        public Stream NewFileStream(string path, FileMode mode)
        {
            throw new NotImplementedException();
        }

        public Stream NewFileStream(string path, FileMode mode, FileAccess access)
        {
            throw new NotImplementedException();
        }

        public Stream OpenRead(string path)
        {
            throw new NotImplementedException();
        }

        public string[] ReadAllLines(string path)
        {
            throw new NotImplementedException();
        }

        public string ReadAllText(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteAllText(string path, string contents)
        {
            throw new NotImplementedException();
        }
    }
}
