// See the LICENSE.TXT file in the project root for full license information.

using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class CodeQualityToolsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public CodeQualityToolsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be use SonarAnalyzer as default tool")]
        public void ShouldBeUseStyleCopAsDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("UsingToolSonarAnalyzer")
                .ShouldEvaluatedEquivalentTo(true);
        }
    }
}
