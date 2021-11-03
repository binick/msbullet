// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class TestProjectFixture : IDisposable
    {
        private readonly ConcurrentQueue<IDisposable> disposables = new ConcurrentQueue<IDisposable>();
        private readonly string testAssets;
        private readonly string boilerplateDir;
        private readonly VisualStudioInstance vsInstance;

        public TestProjectFixture()
        {
            this.testAssets = Path.Combine(AppContext.BaseDirectory, "testassets");
            this.boilerplateDir = Path.Combine(this.testAssets, "boilerplate");
            this.vsInstance = MSBuildLocator.RegisterDefaults();
        }

        public Project ProvideProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.GetProject(new XUnitLogger(output), "ClassLib1", globalProperties);
        }

        public Project ProvideUnitTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.GetProject(new XUnitLogger(output), "ClassLib1.Tests", globalProperties);
        }

        public Project ProvideIntegrationTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.GetProject(new XUnitLogger(output), "ClassLib1.IntegrationTests", globalProperties);
        }

        public Project ProvidePerformanceTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.GetProject(new XUnitLogger(output), "ClassLib1.PerformanceTests", globalProperties);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                while (this.disposables.Count > 0)
                {
                    if (this.disposables.TryDequeue(out IDisposable disposable))
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        private Project GetProject(ILogger logger, string projectName, IDictionary<string, string> globalProperties)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var collection = new ProjectCollection(globalProperties ?? new Dictionary<string, string>());
#pragma warning restore CA2000 // Dispose objects before losing scope
            this.disposables.Enqueue(collection);

            collection.RegisterLoggers(new[] { logger });

            return collection.LoadProject(Path.Combine(this.boilerplateDir, $"{projectName}.csproj"));
        }
    }
}
