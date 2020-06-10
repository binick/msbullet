// See the LICENSE.TXT file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MsBullet.Sdk.IntegrationTests.Utilities
{
    internal class MockEngine : IBuildEngine5
    {
        private readonly ITestOutputHelper output;

        public MockEngine()
        {
        }

        public MockEngine(ITestOutputHelper output) => this.output = output;

        public ICollection<BuildMessageEventArgs> Messages { get; } = new List<BuildMessageEventArgs>();

        public ICollection<BuildWarningEventArgs> Warnings { get; } = new List<BuildWarningEventArgs>();

        public ICollection<BuildErrorEventArgs> Errors { get; } = new List<BuildErrorEventArgs>();

        public bool IsRunningMultipleNodes => false;

        public bool ContinueOnError { get; set; }

        public int LineNumberOfTaskNode => 0;

        public int ColumnNumberOfTaskNode => 0;

        public string ProjectFileOfTaskNode => "<test>";

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            this.output?.WriteLine($"error {e.Code}: {e.Message}");
            this.Errors.Add(e);
            if (!this.ContinueOnError)
            {
                throw new XunitException("Task error: " + e.Message);
            }
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            this.output?.WriteLine($"{e.Importance} : {e.Message}");
            this.Messages.Add(e);
        }

        public void LogTelemetry(string eventName, IDictionary<string, string> properties) => this.output?.WriteLine($"telemetry {eventName}: {properties.Aggregate(string.Empty, (sum, piece) => $"{sum}, {piece.Key} = {piece.Value}")}");

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            this.output?.WriteLine($"warning {e.Code} : {e.Message}");
            this.Warnings.Add(e);
        }

        public void LogCustomEvent(CustomBuildEventArgs e) => this.output?.WriteLine($"{e.Message ?? string.Empty}");

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs, string toolsVersion) => throw new System.NotImplementedException();

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) => throw new System.NotImplementedException();

        public BuildEngineResult BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties, IList<string>[] removeGlobalProperties, string[] toolsVersion, bool returnTargetOutputs) => throw new System.NotImplementedException();

        public bool BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties, IDictionary[] targetOutputsPerProject, string[] toolsVersion, bool useResultsCache, bool unloadProjectsOnCompletion) => throw new System.NotImplementedException();

        public object GetRegisteredTaskObject(object key, RegisteredTaskObjectLifetime lifetime) => throw new System.NotImplementedException();

        public void Reacquire() => throw new System.NotImplementedException();

        public void RegisterTaskObject(object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection) => throw new System.NotImplementedException();

        public object UnregisterTaskObject(object key, RegisteredTaskObjectLifetime lifetime) => throw new System.NotImplementedException();

        public void Yield() => throw new System.NotImplementedException();
    }
}
