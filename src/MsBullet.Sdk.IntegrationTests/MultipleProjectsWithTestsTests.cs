// See the LICENSE.TXT file in the project root for full license information.

using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    [Collection(TestProjectCollection.Name)]
    public class MultipleProjectsWithTestsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public MultipleProjectsWithTestsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("Debug", "", "index.html", "Summary.tex", "Cobertura.xml")]
        [InlineData("Release", "", "index.html", "Summary.tex", "Cobertura.xml")]
        [InlineData("Debug", "Html", "index.html", "Cobertura.xml")]
        [InlineData("Release", "Html", "index.html", "Cobertura.xml")]
        public void MinimalRepoRunTestsShoudGenerateCoverageReportSummary(string configuration, string reportTypes, params string[] reports)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MultipleProjectsWithTests").Create(this.output);
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration, "Reports", "Summary");
            var args = new[]
            {
                "-test",
                $"-configuration {configuration}"
            };
            if (!string.IsNullOrWhiteSpace(reportTypes))
            {
                args = args.Append($"/p:ReportTypes=\"{reportTypes}\"").ToArray();
            }

            // When
            int exitCode = app.ExecuteBuild(this.output, args);

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));

            var outDirPahts = Directory.EnumerateFiles(outDir).Select(path => Path.GetFileName(path));
            foreach (var report in reports)
            {
                Assert.Contains(report, outDirPahts);
            }
        }
    }
}
