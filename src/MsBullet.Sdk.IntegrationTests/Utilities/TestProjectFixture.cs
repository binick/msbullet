// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using MsBullet.Sdk.IntegrationTests.Utilities;
using Xunit;

namespace MsBullet.Sdk.IntegrationTests
{
    [Collection(TestProjectCollection.Name)]
    public class TestProjectFixture : IDisposable
    {
        private static readonly string[] packagesToClear =
        {
            "MsBullet.Sdk",
        };

        private readonly ConcurrentQueue<IDisposable> disposables = new ConcurrentQueue<IDisposable>();
        private readonly string logOutputDir;
        private readonly string testAssets;
        private readonly string boilerplateDir;

        public TestProjectFixture()
        {
            this.ClearPackages();
            this.logOutputDir = this.GetType().Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(m => m.Key == "LogOutputDir").Value;
            this.testAssets = Path.Combine(AppContext.BaseDirectory, "testassets");
            this.boilerplateDir = Path.Combine(this.testAssets, "boilerplate");
        }

        public TestAppProvider ProvideTestApp(string name)
        {
            string testAppFiles = Path.Combine(this.testAssets, name);
            string instanceName = Path.GetRandomFileName();
            string tempDir = Path.Combine(Path.GetTempPath(), "MsBullet", instanceName);

            return new TestAppProvider(tempDir, this.logOutputDir, new[] { testAppFiles, this.boilerplateDir }, this.disposables);
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

        private void ClearPackages()
        {
            string nugetRoot = this.GetType().Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(m => m.Key == "NuGetPackageRoot").Value;
            string pkgVersion = this.GetType().Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(m => m.Key == "PackageVersion").Value;
            foreach (string package in packagesToClear)
            {
                string pkgRoot = Path.Combine(nugetRoot, package, pkgVersion);
                if (Directory.Exists(pkgRoot))
                {
                    Directory.Delete(pkgRoot, recursive: true);
                }
            }
        }
    }
}
