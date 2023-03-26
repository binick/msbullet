// See the LICENSE.TXT file in the project root for full license information.

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    internal sealed class XUnitLogger : ConsoleLogger
    {
        public XUnitLogger(ITestOutputHelper output)
            : this(output, LoggerVerbosity.Normal)
        {
        }

        public XUnitLogger(ITestOutputHelper output, LoggerVerbosity verbosity)
            : base(verbosity)
        {
            this.WriteHandler = output.WriteLine;
        }
    }
}
