// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class ReportCoverageToolsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public ReportCoverageToolsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be use ReportGenerator as default tool")]
        public void ShouldBeUseStyleCopAsDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("UsingToolReportGenerator")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Theory(DisplayName = "Should be at least one output format as default")]
        [InlineData("Html")]
        [InlineData("Latex")]
        public void ShouldBeSetAtLeastOneOutputFormatAsDefault(string expectedFormat)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldCountainProperty("ReportTypes")
                .EvaluatedValue
                .Split(";")
                .Should()
                .Contain(expectedFormat);
        }

        [Fact(DisplayName = "Should be set verbose as default verbosity level")]
        public void ShouldBeSetVerboseAsDefaultVerbosityLevel()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldCountainProperty("VerbosityLevel")
                .ShouldEvaluatedEquivalentTo("Verbose");
        }

        [Fact(DisplayName = "Should not be referenced in a non-test project")]
        public void ShouldNotBeReferencedInANonTestProject()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("ReportGenerator", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should not be reference ReportGenerator package when it is opted out")]
        public void ShouldNotBeReferenceReportGeneratorPackageWhenItIsOptedOut()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "UsingToolReportGenerator", "false" } });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("ReportGenerator", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should be referenced in a test project")]
        public void ShouldBeReferencedInATestProjectWithDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .Contain(i => i.EvaluatedInclude.Equals("ReportGenerator", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should place the output in a directory named")]
        public void ShouldPlaceTheOutputInADirectoryNamed()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project.ShouldCountainProperty("CoverletOutput").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasParent(project.ShouldCountainProperty("ArtifactsCoverageDir").EvaluatedValue);
        }

        [Fact(DisplayName = "Should be respect the version setup")]
        public void ShouldBeRespectTheVersionSetup()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .ContainSingle(i => i.EvaluatedInclude.Equals("ReportGenerator", StringComparison.OrdinalIgnoreCase))
                .Which
                .DirectMetadata
                .Should()
                .ContainSingle(m => m.Name.Equals("Version", StringComparison.OrdinalIgnoreCase))
                .Which
                .UnevaluatedValue
                .Should()
                .Be("$(ReportGeneratorVersion)");
        }

        [Theory(DisplayName = "Should not be define report generation target when it is opted out")]
        [InlineData("ReportCoverage")]
        [InlineData("ReportCoverageSummary")]
        public void ShouldNotBeDefineTargetWhenItIsOptedOut(string target)
        {
            var project = this.fixture.ProvideUnitTestProject(this.output, new Dictionary<string, string>() { { "UsingToolReportGenerator", "false" } });

            project
                .ShouldCountainSingleTarget(target)
                .Value
                .Condition
                .Should()
                .Contain("'$(UsingToolReportGenerator)' == 'true'");
        }
    }
}
