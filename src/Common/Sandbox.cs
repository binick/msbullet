// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests.Utilities
{
    public class Sandbox : IDisposable
    {
        private readonly string logOutputDir;

        public Sandbox(string workDir, string logOutputDir, string[] sourceDirectories)
        {
            if (sourceDirectories == null)
            {
                throw new ArgumentNullException(nameof(sourceDirectories));
            }

            this.WorkingDirectory = workDir;
            this.logOutputDir = Path.Combine(logOutputDir, Path.GetFileName(workDir));

            Directory.CreateDirectory(workDir);
            Directory.CreateDirectory(this.logOutputDir);

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

        protected virtual int Start(ITestOutputHelper output, ProcessStartInfo psi)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (psi == null)
            {
                throw new ArgumentNullException(nameof(psi));
            }

            void Write(object sender, DataReceivedEventArgs e)
            {
                output.WriteLine(e.Data ?? string.Empty);
            }

            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.Environment["PATH"] = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.PathSeparator + Environment.GetEnvironmentVariable("PATH");

            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += Write;
            process.ErrorDataReceived += Write;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit(1000 * 60 * 3);

            process.OutputDataReceived -= Write;
            process.ErrorDataReceived -= Write;
            return process.ExitCode;
        }

        protected virtual int ExecuteScript(ITestOutputHelper output, string fileName, IEnumerable<string> scriptArgs)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (string.IsNullOrEmpty(fileName))
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException("Script file is not valorized.", nameof(fileName));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            if (scriptArgs is null)
            {
                throw new ArgumentNullException(nameof(scriptArgs));
            }

            output.WriteLine("Working dir = " + this.WorkingDirectory);
            output.WriteLine("Log output  = " + this.logOutputDir);
            output.WriteLine($"Run command  = {fileName} {string.Join(' ', scriptArgs)}");

            string cmd = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "cmd.exe"
                    : "bash";

            var psi = new ProcessStartInfo
            {
                FileName = cmd,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,

                WorkingDirectory = this.WorkingDirectory,
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                psi.ArgumentList.Add("/C");
            }

            psi.ArgumentList.Add(fileName);

            foreach (string scriptArg in scriptArgs)
            {
                psi.ArgumentList.Add(scriptArg);
            }

            return this.Start(output, psi);
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
