// See the LICENSE.TXT file in the project root for full license information.

using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    [Collection(TestProjectCollection.Name)]
    public class MinimalRepoWithTestsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public MinimalRepoWithTestsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void MinimalRepoBuildsWithoutErrors()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope.
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(this.output);

            // Then
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void MinimalRepoRunTestsWithoutError()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test");

            // Then
            Assert.Equal(0, exitCode);
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "net7.0", "x64", "html", "xml")]
        [InlineData("Release", "ClassLib1.Tests", "net7.0", "x64", "html", "xml")]
        public void MinimalRepoRunTestsShoudProduceTestsResults(string configuration, string project, string targetFramwork, string architecture, params string[] reportFormats)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration);
            var reports = reportFormats.Select(format => $"{project}_{targetFramwork}_{architecture}.{format}");

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));

            var outDirPahts = Directory.EnumerateFiles(outDir).Select(Path.GetFileName);
            foreach (var report in reports)
            {
                Assert.Contains(report, outDirPahts);
            }
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "net7.0", "x64", "log")]
        [InlineData("Release", "ClassLib1.Tests", "net7.0", "x64", "log")]
        public void MinimalRepoRunTestsShoudProduceLogResults(string configuration, string project, string targetFramwork, string architecture, string fileExtension)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "log", configuration);
            var fileName = $"{project}_{targetFramwork}_{architecture}.{fileExtension}";

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.Contains(fileName, Directory.EnumerateFiles(outDir).Select(Path.GetFileName));
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "xml")]
        [InlineData("Release", "ClassLib1.Tests", "xml")]
        public void MinimalRepoRunTestsShoudCollectCoverageMetrics(string configuration, string project, string fileExtension)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration, "Coverage");
            var fileName = $"{project}.{fileExtension}";

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.Contains(fileName, Directory.EnumerateFiles(outDir).Select(Path.GetFileName));
        }

        [Theory]
        [InlineData("Debug", "ClassLib1.Tests", "index.html")]
        [InlineData("Release", "ClassLib1.Tests", "index.html")]
        [InlineData("Debug", "ClassLib1.Tests", "Summary.tex")]
        [InlineData("Release", "ClassLib1.Tests", "Summary.tex")]
        public void MinimalRepoRunTestsShoudGenerateCoverageReports(string configuration, string project, string fileName)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration, "Reports", project, "Reports");

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.Contains(fileName, Directory.EnumerateFiles(outDir).Select(Path.GetFileName));
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void MinimalRepoRunTestsShoudNotGenerateCoverageSummaryReportsWhenItsPropertyIsFalse(string configuration)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration, "Reports");

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}",
                "/p:GenerateCoverageReportSummary=false");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.DoesNotContain("Summary", Directory.EnumerateDirectories(outDir));
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void MinimalRepoRunTestsShoudNotGenerateCoverageReportsWhenReportGeneratorToolIsOptedOut(string configuration)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration);

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}",
                "/p:UsingToolReportGenerator=false");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.DoesNotContain("Reports", Directory.EnumerateDirectories(outDir));
        }

        [Theory]
        [InlineData(false, "NonShippable")]
        [InlineData(true, "Shippable")]
        public void MinimalRepoPackWithoutErrors(bool isShippable, string destinationFolder)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MinimalRepoWithTests")
                .WithPreCreate("git", "init")
                .WithPreCreate("git", "remote", "add", "origin", "http://localhost")
                .WithPreCreate("git", "checkout", "-b", "main")
                .WithPreCreate("git", "commit", "--allow-empty", "-m", "Dummy happy empty commit.")
                .Create(this.output);
            var expectedVersion = "1.0.0-local.*";
            var packageFileName = $"ClassLib1.{expectedVersion}.nupkg";

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                "-pack",
                $"/p:IsShippable={isShippable}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.Single(Directory.GetFiles(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", destinationFolder), packageFileName));
        }
    }
}
