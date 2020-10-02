// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Coverlet.Core;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Enums;
using Coverlet.Core.Reporters;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace MsBullet.Build.Tasks.Coverlet
{
    public class CoverletGenerateReportTask : TaskBase
    {
        private readonly CoverletLogger logger;

        public CoverletGenerateReportTask()
        {
            this.logger = new CoverletLogger(this.Log);
        }

        [Required]
        public string Output { get; set; }

        [Required]
        public string OutputFormat { get; set; }

        [Required]
        public double Threshold { get; set; }

        [Required]
        public string ThresholdType { get; set; }

        [Required]
        public string ThresholdStat { get; set; }

        [Required]
        public ITaskItem InstrumenterState { get; set; }

        public string MultiTargetFrameworksCurrentTFM { get; set; }

        public override bool Execute()
        {
            try
            {
                IFileSystem fileSystem = Container.GetService<IFileSystem>();
                if (this.InstrumenterState is null || !fileSystem.Exists(this.InstrumenterState.ItemSpec))
                {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    this.logger.LogError("Result of instrumentation task not found");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                    return false;
                }

                Coverage coverage = null;
                using (Stream instrumenterStateStream = fileSystem.NewFileStream(this.InstrumenterState.ItemSpec, FileMode.Open))
                {
                    var instrumentationHelper = Container.GetService<IInstrumentationHelper>();

                    // Task.Log is teared down after a task and thus the new MSBuildLogger must be passed to the InstrumentationHelper
                    // https://github.com/microsoft/msbuild/issues/5153
                    instrumentationHelper.SetLogger(this.logger);
                    coverage = new Coverage(
                        CoveragePrepareResult.Deserialize(instrumenterStateStream),
                        this.logger,
                        Container.GetService<IInstrumentationHelper>(),
                        fileSystem);
                }

                try
                {
                    fileSystem.Delete(this.InstrumenterState.ItemSpec);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // We don't want to block coverage for I/O errors
                    this.logger.LogWarning($"Exception during instrument state deletion, file name '{this.InstrumenterState.ItemSpec}' exception message '{ex.Message}'");
                }

                CoverageResult result = coverage.GetCoverageResult();

                var directory = Path.GetDirectoryName(this.Output);
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Directory.GetCurrentDirectory();
                }
                else if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var formats = this.OutputFormat.Split(',', ';');
                foreach (var format in formats)
                {
                    var reporter = new ReporterFactory(format).CreateReporter();
                    if (reporter == null)
                    {
                        throw new Exception($"Specified output format '{format}' is not supported");
                    }

                    switch (reporter.OutputType)
                    {
                        case ReporterOutputType.Console:
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                            throw new NotSupportedException("Console reporter is not supported");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                        default:
                            new ReportWriter(
                                this.MultiTargetFrameworksCurrentTFM,
                                directory,
                                this.Output,
                                reporter,
                                fileSystem,
                                Container.GetService<IConsole>(),
                                result).WriteReport();
                            break;
                    }
                }

                var thresholdTypeFlags = ThresholdTypeFlags.None;
                var thresholdStat = ThresholdStatistic.Minimum;

                foreach (var thresholdType in this.ThresholdType.Split(',', ';').Select(t => t.Trim()))
                {
                    if (thresholdType.Equals("line", StringComparison.OrdinalIgnoreCase))
                    {
                        thresholdTypeFlags |= ThresholdTypeFlags.Line;
                    }
                    else if (thresholdType.Equals("branch", StringComparison.OrdinalIgnoreCase))
                    {
                        thresholdTypeFlags |= ThresholdTypeFlags.Branch;
                    }
                    else if (thresholdType.Equals("method", StringComparison.OrdinalIgnoreCase))
                    {
                        thresholdTypeFlags |= ThresholdTypeFlags.Method;
                    }
                }

                if (this.ThresholdStat.Equals("average", StringComparison.OrdinalIgnoreCase))
                {
                    thresholdStat = ThresholdStatistic.Average;
                }
                else if (this.ThresholdStat.Equals("total", StringComparison.OrdinalIgnoreCase))
                {
                    thresholdStat = ThresholdStatistic.Total;
                }

                thresholdTypeFlags = result.GetThresholdTypesBelowThreshold(new CoverageSummary(), this.Threshold, thresholdTypeFlags, thresholdStat);
                if (thresholdTypeFlags != ThresholdTypeFlags.None)
                {
                    var exceptionMessageBuilder = new StringBuilder();
                    if ((thresholdTypeFlags & ThresholdTypeFlags.Line) != ThresholdTypeFlags.None)
                    {
#pragma warning disable CA1308 // Normalize strings to uppercase
                        exceptionMessageBuilder.AppendLine($"The {thresholdStat.ToString().ToLower(CultureInfo.InvariantCulture)} line coverage is below the specified {this.Threshold}");
#pragma warning restore CA1308 // Normalize strings to uppercase
                    }

                    if ((thresholdTypeFlags & ThresholdTypeFlags.Branch) != ThresholdTypeFlags.None)
                    {
#pragma warning disable CA1308 // Normalize strings to uppercase
                        exceptionMessageBuilder.AppendLine($"The {thresholdStat.ToString().ToLower(CultureInfo.InvariantCulture)} branch coverage is below the specified {this.Threshold}");
#pragma warning restore CA1308 // Normalize strings to uppercase
                    }

                    if ((thresholdTypeFlags & ThresholdTypeFlags.Method) != ThresholdTypeFlags.None)
                    {
#pragma warning disable CA1308 // Normalize strings to uppercase
                        exceptionMessageBuilder.AppendLine($"The {thresholdStat.ToString().ToLower(CultureInfo.InvariantCulture)} method coverage is below the specified {this.Threshold}");
#pragma warning restore CA1308 // Normalize strings to uppercase
                    }

                    throw new ThresholdException(exceptionMessageBuilder.ToString());
                }
            }
            catch (ThresholdException ex)
            {
                this.logger.LogWarning(ex.Message);
            }

            return true;
        }
    }
}
