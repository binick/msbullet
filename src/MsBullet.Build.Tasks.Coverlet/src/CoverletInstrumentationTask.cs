// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.IO;
using Coverlet.Core;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;

using ILogger = Coverlet.Core.Abstractions.ILogger;

namespace MsBullet.Build.Tasks.Coverlet
{
    public sealed class CoverletInstrumentationTask : TaskBase
    {
        private readonly CoverletLogger logger;

        public CoverletInstrumentationTask()
        {
            this.logger = new CoverletLogger(this.Log);
        }

        [Required]
        public string Path { get; set; }

        [Required]
        public string InstrumenterStatePath { get; set; }

        public string Include { get; set; }

        public string IncludeDirectory { get; set; }

        public string Exclude { get; set; }

        public string ExcludeByFile { get; set; }

        public string ExcludeByAttribute { get; set; }

        public bool IncludeTestAssembly { get; set; }

        public bool SingleHit { get; set; }

        public string MergeWith { get; set; }

        public bool UseSourceLink { get; set; } = true;

        public bool SkipAutoProps { get; set; }

        [Output]
        public ITaskItem InstrumenterState { get; set; }

        public override bool Execute()
        {
            Container = this.WireUp(this.Path).BuildServiceProvider();

            var fileSystem = Container.GetService<IFileSystem>();

            CoveragePrepareResult coverageResult;
            try
            {
                var coverage = new Coverage(
                    this.Path,
                    this.Include?.Split(',', ';'),
                    this.IncludeDirectory?.Split(',', ';'),
                    this.Exclude?.Split(',', ';'),
                    this.ExcludeByFile?.Split(',', ';'),
                    this.ExcludeByAttribute?.Split(',', ';'),
                    this.IncludeTestAssembly,
                    this.SingleHit,
                    this.MergeWith,
                    this.UseSourceLink,
                    this.logger,
                    Container.GetService<IInstrumentationHelper>(),
                    Container.GetService<IFileSystem>(),
                    Container.GetService<ISourceRootTranslator>());

                coverageResult = coverage.PrepareModules();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.logger.LogError(ex);
                return false;
            }

            this.InstrumenterState = new TaskItem(this.InstrumenterStatePath);

            try
            {
                using var instrumentedStateFile = fileSystem.NewFileStream(this.InstrumenterState.ItemSpec, FileMode.Open, FileAccess.Write);
                using Stream serializedState = CoveragePrepareResult.Serialize(coverageResult);
                serializedState.CopyTo(instrumentedStateFile);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.logger.LogError(ex);
                return false;
            }

            return true;
        }

        private IServiceCollection WireUp(string modulePath)
        {
            return new ServiceCollection()
                .AddTransient<IProcessExitHandler, ProcessExitHandler>()
                .AddTransient<IFileSystem, FileSystem>()
                .AddTransient<IConsole, SystemConsole>()
                .AddTransient<ILogger, CoverletLogger>(_ => this.logger)
                .AddTransient<IRetryHelper, RetryHelper>()
                .AddSingleton<ISourceRootTranslator, SourceRootTranslator>(serviceProvider => new SourceRootTranslator(
                    modulePath,
                    serviceProvider.GetRequiredService<ILogger>(),
                    serviceProvider.GetRequiredService<IFileSystem>()))
                .AddSingleton<IInstrumentationHelper, InstrumentationHelper>();
        }
    }
}
