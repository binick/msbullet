// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MsBullet.Sdk.IntegrationTests.Utilities;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    public class TestApp : Sandbox
    {
        private readonly string logOutputDir;

        public TestApp(string workDir, string logOutputDir, string[] sourceDirectories)
            : base(workDir, logOutputDir, sourceDirectories)
        {
            this.logOutputDir = Path.Combine(logOutputDir, Path.GetFileName(workDir));
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
