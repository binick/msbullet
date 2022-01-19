// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MsBullet.Sdk.Tests.Utilities;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    public class TestApp : Sandbox
    {
        private readonly string logOutputDir;

        public TestApp(string workDir, string logOutputDir, string[] sourceDirectories)
            : base(workDir, sourceDirectories)
        {
            this.logOutputDir = Path.Combine(logOutputDir, Path.GetFileName(workDir));
            Directory.CreateDirectory(this.logOutputDir);
        }

        public event EventHandler WireUp;

        public event EventHandler PreBuild;

        public event EventHandler PostBuild;

        public int ExecuteBuild(ITestOutputHelper output, params string[] scriptArgs)
        {
            this.WireUp?.Invoke(this, EventArgs.Empty);
            this.PreBuild?.Invoke(this, EventArgs.Empty);

            int result = this.ExecuteScript(
                output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @".\build.cmd" : "./build.sh",
                scriptArgs.Append(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-binaryLog" : "--binaryLog"));

            Copy(Path.Combine(this.WorkingDirectory, "artifacts", "log"), this.logOutputDir);

            this.PostBuild?.Invoke(this, EventArgs.Empty);

            return result;
        }

        public int UsafeExecutionScript(ITestOutputHelper output, string command, params string[] commandArgs)
        {
            int result = this.ExecuteScript(output, command, commandArgs);

            return result;
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

        private static void Copy(string srcDir, string destDir)
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
    }
}
