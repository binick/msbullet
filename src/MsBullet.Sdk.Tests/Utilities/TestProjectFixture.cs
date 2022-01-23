// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
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

        public TestProjectFixture()
        {
            this.testAssets = Path.Combine(AppContext.BaseDirectory, "testassets");
            this.boilerplateDir = Path.Combine(this.testAssets, "boilerplate");
            _ = MSBuildLocator.RegisterDefaults();
        }

        public Project ProvideProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.InternalProvideProject(output, "ClassLib1", globalProperties ?? new Dictionary<string, string>());
        }

        public Project ProvideProject(ITestOutputHelper output, string projectName, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.InternalProvideProject(output, projectName, globalProperties ?? new Dictionary<string, string>());
        }

        public Project ProvideUnitTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.InternalProvideProject(output, "ClassLib1.Tests", globalProperties ?? new Dictionary<string, string>());
        }

        public Project ProvideIntegrationTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.InternalProvideProject(output, "ClassLib1.IntegrationTests", globalProperties ?? new Dictionary<string, string>());
        }

        public Project ProvidePerformanceTestProject(ITestOutputHelper output, IDictionary<string, string> globalProperties = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return this.InternalProvideProject(output, "ClassLib1.PerformanceTests", globalProperties ?? new Dictionary<string, string>());
        }

        public Project ProvideToolsProject(ITestOutputHelper output, string relativePath, IDictionary<string, string> globalProperties = null)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var collection = new ProjectCollection(globalProperties ?? new Dictionary<string, string>());
#pragma warning restore CA2000 // Dispose objects before losing scope
            collection.RegisterLoggers(new[] { new XUnitLogger(output) });
            this.disposables.Enqueue(collection);
            return collection.LoadProject(Path.Combine(this.boilerplateDir, "sdk", "tools", relativePath));
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

        private Project InternalProvideProject(ITestOutputHelper output, string projectName, IDictionary<string, string> globalProperties)
        {
            string testAppFiles = Path.Combine(this.testAssets, projectName);
            string instanceName = Path.GetRandomFileName();
            string tempDir = Path.Combine(Path.GetTempPath(), "MsBullet", instanceName);

#pragma warning disable CA2000 // Dispose objects before losing scope
            var testApp = new TestApp(tempDir, new[] { testAppFiles, this.boilerplateDir }, output, globalProperties ?? new Dictionary<string, string>());
#pragma warning restore CA2000 // Dispose objects before losing scope

            this.disposables.Enqueue(testApp);

            return testApp.Project;
        }
    }
}
