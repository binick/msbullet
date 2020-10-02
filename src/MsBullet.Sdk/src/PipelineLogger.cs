// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

namespace MsBullet.Build.Tasks
{
#pragma warning disable SA1402 // File may only contain a single type
    internal enum State
#pragma warning restore SA1402 // File may only contain a single type
    {
        Unknown,
        Initialized,
        InProgress,
        Completed,
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal enum Result
#pragma warning restore SA1402 // File may only contain a single type
    {
        Succeeded,
        SucceededWithIssues,
        Failed,
        Canceled,
        Skipped,
    }

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
    internal readonly struct TelemetryTaskInfo
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
    {
        internal TelemetryTaskInfo(Guid id, string category)
        {
            this.Id = id;
            this.Category = category;
        }

        internal Guid Id { get; }

        internal string Category { get; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal readonly struct ProjectInfo
#pragma warning restore SA1402 // File may only contain a single type
    {
        internal ProjectInfo(string name, Guid? parentId, string propertiesCategory)
        {
            this.Name = name;
            this.Id = Guid.NewGuid();
            this.ParentId = parentId;
            this.StartTime = DateTimeOffset.UtcNow;
            this.PropertiesCategory = propertiesCategory;
        }

        internal string Name { get; }

        internal Guid Id { get; }

        internal Guid? ParentId { get; }

        internal DateTimeOffset StartTime { get; }

        internal string PropertiesCategory { get; }
    }

    /// <summary>
    /// Logger for converting MSBuild error messages to the Azure Pipelines Tasks format.
    /// </summary>
    public sealed class PipelineLogger : ILogger
    {
        private const string TelemetryMarker = "NETCORE_ENGINEERING_TELEMETRY";
        private readonly MessageBuilder builder = new MessageBuilder();
        private readonly Dictionary<BuildEventContext, Guid> buildEventContextMap = new Dictionary<BuildEventContext, Guid>(BuildEventContextComparer.Instance);
        private readonly Dictionary<Guid, ProjectInfo> projectInfoMap = new Dictionary<Guid, ProjectInfo>();
        private readonly Dictionary<Guid, TelemetryTaskInfo> taskTelemetryInfoMap = new Dictionary<Guid, TelemetryTaskInfo>();
        private readonly HashSet<Guid> detailedLoggedSet = new HashSet<Guid>();
        private HashSet<string> ignoredTargets;
        private string solutionDirectory;

        /// <inheritdoc/>
        public LoggerVerbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public string Parameters { get; set; }

        /// <inheritdoc/>
        public void Initialize(IEventSource eventSource)
        {
            if (eventSource is null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }

            var parameters = LoggerParameters.Parse(this.Parameters);

            this.solutionDirectory = parameters["SolutionDir"];

            var verbosityString = parameters["Verbosity"];
            this.Verbosity = !string.IsNullOrEmpty(verbosityString) && Enum.TryParse(verbosityString, out LoggerVerbosity verbosity)
                ? verbosity
                : LoggerVerbosity.Normal;

            var ignoredTargets = new string[]
            {
                "GetCopyToOutputDirectoryItems",
                "GetNativeManifest",
                "GetTargetPath",
                "GetTargetFrameworks",
            };
            this.ignoredTargets = new HashSet<string>(ignoredTargets, StringComparer.OrdinalIgnoreCase);

            // TargetsNotLogged is an optional parameter.
            var targetsNotLogged = parameters["TargetsNotLogged"];
            if (!string.IsNullOrEmpty(targetsNotLogged))
            {
                this.ignoredTargets.UnionWith(targetsNotLogged.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            eventSource.ErrorRaised += this.OnErrorRaised;
            eventSource.WarningRaised += this.OnWarningRaised;
            eventSource.ProjectStarted += this.OnProjectStarted;

            IEventSource2 eventSource2 = (IEventSource2)eventSource;
            eventSource2.TelemetryLogged += this.OnTelemetryLogged;

            if (this.Verbosity == LoggerVerbosity.Diagnostic)
            {
                eventSource.ProjectFinished += this.OnProjectFinished;
            }
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
        }

        private void LogErrorOrWarning(
            bool isError,
            string sourceFilePath,
            int line,
            int column,
            string code,
            string message,
            BuildEventContext buildEventContext)
        {
            var parentId = this.buildEventContextMap.TryGetValue(buildEventContext, out var guid)
                ? (Guid?)guid
                : null;
            string telemetryCategory = null;
            if (parentId.HasValue)
            {
                if (this.taskTelemetryInfoMap.TryGetValue(parentId.Value, out TelemetryTaskInfo telemetryInfo))
                {
                    telemetryCategory = telemetryInfo.Category;
                }

                if (string.IsNullOrEmpty(telemetryCategory))
                {
                    if (this.projectInfoMap.TryGetValue(parentId.Value, out ProjectInfo projectInfo))
                    {
                        telemetryCategory = projectInfo.PropertiesCategory;
                    }
                }
            }

            this.builder.Start("logissue");
            this.builder.AddProperty("type", isError ? "error" : "warning");
            this.builder.AddProperty("sourcepath", sourceFilePath);
            this.builder.AddProperty("linenumber", line);
            this.builder.AddProperty("columnnumber", column);
            this.builder.AddProperty("code", code);
            if (telemetryCategory != null)
            {
                message = $"({TelemetryMarker}={telemetryCategory}) {message}";
            }

            this.builder.Finish(message);
            Console.WriteLine(this.builder.GetMessage());
        }

        private void LogDetail(
            Guid id,
            string type,
            string name,
            State state,
            Result? result = null,
            DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null,
            string order = null,
            string progress = null,
            Guid? parentId = null)
        {
            this.builder.Start("logdetail");
            this.builder.AddProperty("id", id);

            if (parentId != null)
            {
                this.builder.AddProperty("parentid", parentId.Value);
            }

            // Certain values on logdetail can only be set once by design of VSO
            if (this.detailedLoggedSet.Add(id))
            {
                this.builder.AddProperty("type", type);
                this.builder.AddProperty("name", name);

                if (!string.IsNullOrEmpty(order))
                {
                    this.builder.AddProperty("order", order);
                }
            }

            if (startTime.HasValue)
            {
                this.builder.AddProperty("starttime", startTime.Value);
            }

            if (endTime.HasValue)
            {
                this.builder.AddProperty("endtime", endTime.Value);
            }

            if (!string.IsNullOrEmpty(progress))
            {
                this.builder.AddProperty("progress", progress);
            }

            this.builder.AddProperty("state", state.ToString());
            if (result.HasValue)
            {
                this.builder.AddProperty("result", result.Value.ToString());
            }

            this.builder.Finish();

            Console.WriteLine(this.builder.GetMessage());
        }

        private void LogBuildEvent(
            in ProjectInfo projectInfo,
            State state,
            Result? result = null,
            DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null,
            string order = null,
            string progress = null) =>
            this.LogDetail(
                id: projectInfo.Id,
                type: "Build",
                name: projectInfo.Name,
                state: state,
                result: result,
                startTime: startTime,
                endTime: endTime,
                progress: progress,
                order: order,
                parentId: projectInfo.ParentId);

        private void OnErrorRaised(object sender, BuildErrorEventArgs e) =>
            this.LogErrorOrWarning(isError: true, e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, e.BuildEventContext);

        private void OnWarningRaised(object sender, BuildWarningEventArgs e) =>
            this.LogErrorOrWarning(isError: false, e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, e.BuildEventContext);

        private void OnTelemetryLogged(object sender, TelemetryEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.EventName.Equals(TelemetryMarker, StringComparison.OrdinalIgnoreCase))
            {
                if (!e.Properties.TryGetValue("Category", out string telemetryCategory))
                {
                    return;
                }

                if (!this.buildEventContextMap.TryGetValue(e.BuildEventContext, out var parentId))
                {
                    return;
                }

                if (string.IsNullOrEmpty(telemetryCategory))
                {
                    this.taskTelemetryInfoMap.Remove(parentId);
                }
                else
                {
                    var telemetryInfo = new TelemetryTaskInfo(parentId, telemetryCategory);
                    this.taskTelemetryInfoMap[parentId] = telemetryInfo;
                }
            }
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            if (!this.buildEventContextMap.TryGetValue(e.BuildEventContext, out Guid projectId) ||
                !this.projectInfoMap.TryGetValue(projectId, out ProjectInfo projectInfo))
            {
                return;
            }

            this.LogBuildEvent(
                in projectInfo,
                State.Completed,
                result: e.Succeeded ? Result.Succeeded : Result.Failed,
                startTime: projectInfo.StartTime,
                endTime: DateTimeOffset.UtcNow,
                progress: "100");
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            if (this.ignoredTargets.Contains(e.TargetNames))
            {
                return;
            }

            string propertyCategory = e.Properties?.Cast<DictionaryEntry>().LastOrDefault(p => p.Key.ToString().Equals(TelemetryMarker, StringComparison.OrdinalIgnoreCase)).Value?.ToString();
            if (string.IsNullOrWhiteSpace(propertyCategory))
            {
                propertyCategory = e.GlobalProperties?.LastOrDefault(p => p.Key.ToString(CultureInfo.InvariantCulture).Equals($"_{TelemetryMarker}", StringComparison.OrdinalIgnoreCase)).Value;
            }

            var parentId = this.buildEventContextMap.TryGetValue(e.ParentProjectBuildEventContext, out var guid)
                ? (Guid?)guid
                : null;

            var projectInfo = new ProjectInfo(GetName(), parentId, propertyCategory);

            this.buildEventContextMap[e.BuildEventContext] = projectInfo.Id;

            this.projectInfoMap[projectInfo.Id] = projectInfo;

            if (this.Verbosity == LoggerVerbosity.Diagnostic)
            {
                this.LogBuildEvent(
                    in projectInfo,
                    State.Initialized,
                    startTime: projectInfo.StartTime,
                    endTime: null,
                    progress: "0");
            }

            string GetName()
            {
                if (this.Verbosity != LoggerVerbosity.Diagnostic)
                {
                    return string.Empty;
                }

                // Note, website projects (sln file only, no proj file) emit a started event with projectFile == $"{m_solutionDirectory}\\".
                // This causes issues when attempting to get the relative path (and also Path.GetFileName returns empty string).
                var projectFile = e.ProjectFile;
                projectFile = (projectFile ?? string.Empty).TrimEnd('\\');

                // Make the name relative.
                if (!string.IsNullOrEmpty(this.solutionDirectory) &&
                    projectFile.StartsWith(this.solutionDirectory + @"\", StringComparison.OrdinalIgnoreCase))
                {
                    projectFile = projectFile.Substring(this.solutionDirectory.Length).TrimStart('\\');
                }
                else
                {
                    try
                    {
                        projectFile = Path.GetFileName(projectFile);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                // Default the project file.
                if (string.IsNullOrEmpty(projectFile))
                {
                    projectFile = "Unknown";
                }

                string targetFrameworkQualifier = string.Empty;
                if (e.GlobalProperties.TryGetValue("TargetFramework", out string targetFramework))
                {
                    targetFrameworkQualifier = $" - {targetFramework}";
                }

                string targetNamesQualifier = string.IsNullOrEmpty(e.TargetNames) ? string.Empty : $" ({e.TargetNames})";

                return projectFile + targetFrameworkQualifier + targetNamesQualifier;
            }
        }

        internal sealed class LoggerParameters
        {
            internal const char NameValueDelimiter = '=';
            internal const char NameValuePairDelimiter = '|';
            private readonly Dictionary<string, string> parameters;

            public LoggerParameters(Dictionary<string, string> parameters)
            {
                this.parameters = parameters;
            }

            internal static StringComparer Comparer => StringComparer.OrdinalIgnoreCase;

            public string this[string name] => this.parameters.TryGetValue(name, out var value) ? value : string.Empty;

            public static LoggerParameters Parse(string paramString)
            {
                if (string.IsNullOrEmpty(paramString))
                {
                    return new LoggerParameters(new Dictionary<string, string>(Comparer));
                }

                // split the given string into name1 = value1 | name2 = value2
                string[] nameValuePairs = paramString.Split(NameValuePairDelimiter);
                var parameters = new Dictionary<string, string>(Comparer);
                foreach (string str in nameValuePairs)
                {
                    // look for the = char. URI's are value and can have = in them.
#pragma warning disable CA1307 // Specify StringComparison
                    int valueDelimiterIndex = str.IndexOf(NameValueDelimiter);
#pragma warning restore CA1307 // Specify StringComparison
                    if (valueDelimiterIndex >= 0)
                    {
                        // get the 2 strings.
                        string name = str.Substring(0, valueDelimiterIndex);
                        string value = str.Substring(valueDelimiterIndex + 1);
                        parameters.Add(name.Trim(), value.Trim());
                    }
                }

                return new LoggerParameters(parameters);
            }
        }

        internal sealed class MessageBuilder
        {
            private readonly StringBuilder builder = new StringBuilder();
            private State state;

            internal enum State
            {
                NotStarted,
                Properties,
                Finished,
            }

            internal void Start(string kind)
            {
#pragma warning disable SA1405 // Debug.Assert should provide message text
                Debug.Assert(this.state == State.NotStarted || this.state == State.Finished);
#pragma warning restore SA1405 // Debug.Assert should provide message text

                this.builder.Length = 0;
                this.builder.Append($"##vso[task.{kind} ");
                this.state = State.Properties;
            }

            internal void AddProperty(string name, string value)
            {
#pragma warning disable SA1405 // Debug.Assert should provide message text
                Debug.Assert(this.state == State.Properties);
#pragma warning restore SA1405 // Debug.Assert should provide message text

                this.builder.Append($"{name}={Escape(value)};");
            }

            internal void AddProperty(string name, DateTimeOffset value) => this.AddProperty(name, value.ToString("O", CultureInfo.InvariantCulture));

            internal void AddProperty(string name, int value) => this.AddProperty(name, value.ToString(CultureInfo.InvariantCulture));

            internal void AddProperty(string name, Guid value) => this.AddProperty(name, value.ToString("D", CultureInfo.InvariantCulture));

            internal void Finish(string message = null)
            {
#pragma warning disable SA1405 // Debug.Assert should provide message text
                Debug.Assert(this.state == State.Properties);
#pragma warning restore SA1405 // Debug.Assert should provide message text
                this.builder.Append("]");
                if (!string.IsNullOrEmpty(message))
                {
                    this.builder.Append(Escape(message));
                }

                this.state = State.Finished;
            }

            internal string GetMessage()
            {
#pragma warning disable SA1405 // Debug.Assert should provide message text
                Debug.Assert(this.state == State.Finished);
#pragma warning restore SA1405 // Debug.Assert should provide message text
                return this.builder.ToString();
            }

            private static string Escape(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                var result = new StringBuilder(value.Length);
                foreach (char c in value)
                {
                    switch (c)
                    {
                        case ';':
                            result.Append("%3B");
                            break;
                        case '\r':
                            result.Append("%0D");
                            break;
                        case '\n':
                            result.Append("%0A");
                            break;
                        case ']':
                            result.Append("%5D");
                            break;
                        default:
                            result.Append(c);
                            break;
                    }
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Compares two event contexts on ProjectContextId and NodeId only.
        /// </summary>
        internal sealed class BuildEventContextComparer : IEqualityComparer<BuildEventContext>
        {
            public static BuildEventContextComparer Instance { get; } = new BuildEventContextComparer();

            public bool Equals(BuildEventContext x, BuildEventContext y) =>
                x.NodeId == y.NodeId &&
                x.ProjectContextId == y.ProjectContextId;

            // This gives the low 24 bits to ProjectContextId and the high 8 to NodeId.
            public int GetHashCode(BuildEventContext x) => x.ProjectContextId + (x.NodeId << 24);
        }
    }
}
