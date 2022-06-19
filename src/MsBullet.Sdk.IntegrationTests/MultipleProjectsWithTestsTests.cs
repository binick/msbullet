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
        [InlineData("Debug", "index.html")]
        [InlineData("Release", "index.html")]
        [InlineData("Debug", "Summary.tex")]
        [InlineData("Release", "Summary.tex")]
        public void MinimalRepoRunTestsShoudGenerateCoverageReportSummary(string configuration, string fileName)
        {
            // Given
            TestApp app = this.fixture.ProvideTestApp("MultipleProjectsWithTests").Create(this.output);
            var outDir = Path.Combine(app.WorkingDirectory, "artifacts", "TestResults", configuration, "Reports", "Summary");

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-test",
                $"-configuration {configuration}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(Directory.Exists(outDir));
            Assert.Contains(fileName, Directory.EnumerateFiles(outDir).Select(path => Path.GetFileName(path)));
        }
    }
}
