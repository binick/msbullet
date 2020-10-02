// See the LICENSE.TXT file in the project root for full license information.

using System.IO;

using Coverlet.Core;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Reporters;

namespace MsBullet.Build.Tasks.Coverlet
{
    internal class ReportWriter
    {
        private readonly string coverletMultiTargetFrameworksCurrentTFM;
        private readonly string directory;
        private readonly string output;
        private readonly IReporter reporter;
        private readonly IFileSystem fileSystem;
        private readonly IConsole console;
        private readonly CoverageResult result;

        public ReportWriter(
            string coverletMultiTargetFrameworksCurrentTFM,
            string directory,
            string output,
            IReporter reporter,
            IFileSystem fileSystem,
            IConsole console,
            CoverageResult result)
        {
            this.coverletMultiTargetFrameworksCurrentTFM = coverletMultiTargetFrameworksCurrentTFM;
            this.directory = directory;
            this.output = output;
            this.reporter = reporter;
            this.result = result;
            this.fileSystem = fileSystem;
            this.console = console;
        }

        public void WriteReport()
        {
            string filename = Path.GetFileName(this.output);

            string separatorPoint = string.IsNullOrEmpty(this.coverletMultiTargetFrameworksCurrentTFM) ? string.Empty : ".";

            if (string.IsNullOrEmpty(filename))
            {
                // empty filename for instance only directory is passed to CoverletOutput c:\reportpath
                // c:\reportpath\coverage.reportedextension
                filename = $"coverage.{this.coverletMultiTargetFrameworksCurrentTFM}{separatorPoint}{this.reporter.Extension}";
            }
            else if (Path.HasExtension(filename))
            {
                // filename with extension for instance c:\reportpath\file.ext
                // we keep user specified name
                filename = $"{Path.GetFileNameWithoutExtension(filename)}{separatorPoint}{this.coverletMultiTargetFrameworksCurrentTFM}{Path.GetExtension(filename)}";
            }
            else
            {
                // filename without extension for instance c:\reportpath\file
                // c:\reportpath\file.reportedextension
                filename = $"{filename}{separatorPoint}{this.coverletMultiTargetFrameworksCurrentTFM}.{this.reporter.Extension}";
            }

            string report = Path.Combine(this.directory, filename);
            this.console.WriteLine($"  Generating report '{report}'");
            this.fileSystem.WriteAllText(report, this.reporter.Report(this.result));
        }
    }
}
