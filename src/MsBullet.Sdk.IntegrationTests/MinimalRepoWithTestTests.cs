// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    [Collection(TestProjectCollection.Name)]
    public class MinimalRepoWithTestTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public MinimalRepoWithTestTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void MinimalRepoRunTestsWithoutError()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepoWithTests");
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-test" : "--test");

            // Then
            Assert.Equal(0, exitCode);
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "netcoreapp2.1", "x64", "html", "xml")]
        [InlineData("Release", "ClassLib1.Tests", "netcoreapp2.1", "x64", "html", "xml")]
        public void MinimalRepoRunTestsShoudProduceTestsResults(string configuration, string project, string targetFramwork, string architecture, params string[] reportFormats)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepoWithTests");
#pragma warning restore CA2000 // Dispose objects before losing scope
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration);
            var reports = reportFormats.Select(format => $"{project}_{targetFramwork}_{architecture}.{format}");

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-test" : "--test",
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"-configuration {configuration}" : $"--configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));

            var outDirPahts = Directory.EnumerateFiles(outDir).Select(path => Path.GetFileName(path));
            foreach (var report in reports)
            {
                Assert.Contains(report, outDirPahts);
            }
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "netcoreapp2.1", "x64", "log")]
        [InlineData("Release", "ClassLib1.Tests", "netcoreapp2.1", "x64", "log")]
        public void MinimalRepoRunTestsShoudProduceLogResults(string configuration, string project, string targetFramwork, string architecture, string fileExtension)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepoWithTests");
#pragma warning restore CA2000 // Dispose objects before losing scope
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "log", configuration);
            var fileName = $"{project}_{targetFramwork}_{architecture}.{fileExtension}";

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-test" : "--test",
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"-configuration {configuration}" : $"--configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.Contains(fileName, Directory.EnumerateFiles(outDir).Select(path => Path.GetFileName(path)));
        }
    }
}
