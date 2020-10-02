// See the LICENSE.TXT file in the project root for full license information.

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using ILogger = Coverlet.Core.Abstractions.ILogger;

namespace MsBullet.Build.Tasks.Coverlet
{
    public class CoverletLogger : ILogger
    {
        private const string LogPrefix = "[coverlet] ";

        private readonly TaskLoggingHelper logger;

        public CoverletLogger(TaskLoggingHelper logger)
        {
            this.logger = logger;
        }

        public void LogError(string message)
        {
            this.logger.LogError($"{LogPrefix}.{message}");
        }

        public void LogError(Exception exception)
        {
            this.logger.LogErrorFromException(exception, true);
        }

        public void LogInformation(string message, bool important = false)
        {
            var importanceLevel = MessageImportance.Normal;
            if (important)
            {
                importanceLevel = MessageImportance.High;
            }

            this.logger.LogMessage(importanceLevel, $"{LogPrefix}.{message}");
        }

        public void LogVerbose(string message)
        {
            this.logger.LogMessage(MessageImportance.Low, $"{LogPrefix}.{message}");
        }

        public void LogWarning(string message)
        {
            this.logger.LogWarning($"{LogPrefix}.{message}");
        }
    }
}
