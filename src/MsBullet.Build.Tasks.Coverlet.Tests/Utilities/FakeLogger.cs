// See the LICENSE.TXT file in the project root for full license information.

using System;
using Coverlet.Core.Abstractions;
using Xunit.Abstractions;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    internal class FakeLogger : ILogger
    {
        private readonly ITestOutputHelper output;

        public FakeLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void LogError(string message)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string message, bool important = false)
        {
            throw new NotImplementedException();
        }

        public void LogVerbose(string message)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string message)
        {
            throw new NotImplementedException();
        }
    }
}
