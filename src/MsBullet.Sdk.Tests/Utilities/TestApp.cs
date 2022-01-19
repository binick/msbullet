// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using MsBullet.Sdk.Tests.Utilities;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    public class TestApp : Sandbox
    {
        private readonly ProjectCollection collection;

        public TestApp(string workDir, string[] sourceDirectories, ITestOutputHelper output, IDictionary<string, string> globalProperties)
            : base(workDir, sourceDirectories)
        {
            this.collection = new ProjectCollection(globalProperties ?? new Dictionary<string, string>());
            this.collection.RegisterLoggers(new[] { new XUnitLogger(output) });
            this.Project = this.collection.LoadProject(Directory.GetFiles(workDir).Single(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)));
        }

        public Project Project { get; }

        protected override void Dispose(bool disposing)
        {
            this.collection.Dispose();
            base.Dispose(disposing);
        }
    }
}
