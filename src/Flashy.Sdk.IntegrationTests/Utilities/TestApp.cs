// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Flashy.Sdk.IntegrationTests.Utilities;
using Xunit.Abstractions;

namespace Flashy.Sdk.IntegrationTests
{
    public class TestApp : Sandbox
    {
        private readonly string logOutputDir;

        public TestApp(string workDir, string logOutputDir, string[] sourceDirectories)
            : base(workDir, logOutputDir, sourceDirectories) => this.logOutputDir = Path.Combine(logOutputDir, Path.GetFileName(workDir));

        public int ExecuteBuild(ITestOutputHelper output, params string[] scriptArgs)
        {
            this.ExecuteScript(
                output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @".\setup.cmd" : "./setup.sh",
                Array.Empty<string>());

            foreach (var repoSetup in Directory.GetFiles(this.WorkingDirectory, "setup.*"))
            {
                File.Delete(repoSetup);
            }

            int result = this.ExecuteScript(
                output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @".\build.cmd" : "./build.sh",
                scriptArgs.Append(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-binaryLog" : "--binaryLog"));

            Copy(Path.Combine(this.WorkingDirectory, "artifacts", "log"), this.logOutputDir);

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
