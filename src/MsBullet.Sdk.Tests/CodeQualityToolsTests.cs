// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
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

        [Fact(DisplayName = "Should be use Sonar as default tool")]
        public void ShouldBeUseSonarAsDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("UsingToolSonarAnalyzer")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact(DisplayName = "Should be referenced SonarAnalyzer package")]
        public void ShouldBeReferencedSonarAnalyzerPackage()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .Contain(i => i.EvaluatedInclude.Equals("SonarAnalyzer.CSharp", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should not be referenced SonarAnalyzer package when it is opted out")]
        public void ShouldNotBeReferencedSonarAnalyzerPackageWhenItIsOptedOut()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "UsingToolSonarAnalyzer", "false" }
            });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("SonarAnalyzer.CSharp", StringComparison.OrdinalIgnoreCase));
        }
    }
}
