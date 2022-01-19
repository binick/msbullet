// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.IO;
using System.Threading;

namespace MsBullet.Sdk.Tests.Utilities
{
    public class Sandbox : IDisposable
    {
        public Sandbox(string workDir, string[] sourceDirectories)
        {
            if (sourceDirectories == null)
            {
                throw new ArgumentNullException(nameof(sourceDirectories));
            }

            this.WorkingDirectory = workDir;

            Directory.CreateDirectory(workDir);

            foreach (string dir in sourceDirectories)
            {
                this.CopyRecursive(dir, workDir);
            }
        }

        public string WorkingDirectory { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    this.DeleteRecursive(this.WorkingDirectory);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // Sometimes antivirus scanning locks files and they can't be deleted. Retring after 500ms seems to get around this most of the time
                    Thread.Sleep(500);
                    Directory.Delete(this.WorkingDirectory, recursive: true);
                }
            }
        }

        private void CopyRecursive(string srcDir, string destDir)
        {
            foreach (string srcFileName in Directory.EnumerateFiles(srcDir, "*", SearchOption.AllDirectories))
            {
                string destFileName = Path.Combine(destDir, srcFileName.Substring(srcDir.Length).TrimStart(new[]
                {
                    Path.AltDirectorySeparatorChar,
                    Path.DirectorySeparatorChar
                }));

                Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
                File.Copy(srcFileName, destFileName);
            }
        }

        private void DeleteRecursive(string directory)
        {
            foreach (string subdirectory in Directory.EnumerateDirectories(directory))
            {
                this.DeleteRecursive(subdirectory);
            }

            foreach (string fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    Attributes = FileAttributes.Normal
                };
                fileInfo.Delete();
            }

            Directory.Delete(directory);
        }
    }
}
