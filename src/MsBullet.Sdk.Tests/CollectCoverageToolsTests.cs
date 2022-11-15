// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class CollectCoverageToolsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public CollectCoverageToolsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be use Coverlet as default tool")]
        public void ShouldBeUseStyleCopAsDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("UsingToolCoverlet")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact(DisplayName = "Should be use cobertura as default output format")]
        public void ShouldBeUseCoberturaAsDefaultOutputFormat()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldCountainProperty("CoverletOutputFormat")
                .ShouldEvaluatedEquivalentTo("cobertura");
        }

        [Fact(DisplayName = "Should not be referenced in a non-test project")]
        public void ShouldNotBeReferencedInANonTestProject()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("coverlet.msbuild", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should not be reference coverlet.msbuild package when it is opted out")]
        public void ShouldNotBeReferenceCoverletPackageWhenItIsOptedOut()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "UsingToolCoverlet", "false" } });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("coverlet.msbuild", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should be referenced in a test project")]
        public void ShouldBeReferencedInATestProjectWithDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project
                .ShouldCountainProperty("CollectCoverage")
                .ShouldEvaluatedEquivalentTo(true);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .Contain(i => i.EvaluatedInclude.Equals("coverlet.msbuild", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should place the output in a directory named")]
        public void ShouldPlaceTheOutputInADirectoryNamed()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>() { { "IsTestProject", "true" } });

            project.ShouldCountainProperty("CoverletOutput").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasParent(project.ShouldCountainProperty("ArtifactsCoverageDir").EvaluatedValue);
        }
    }
}
