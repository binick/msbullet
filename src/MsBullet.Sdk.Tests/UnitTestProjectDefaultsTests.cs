// See the LICENSE.TXT file in the project root for full license information.

using System.IO;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class UnitTestProjectDefaultsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public UnitTestProjectDefaultsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        public static TheoryData<string> UnitTestProjectWhenEnds => new TheoryData<string>
        {
            "ClassLib1.Tests",
            "ClassLib1.UnitTests"
        };

        [Theory]
        [MemberData(nameof(UnitTestProjectWhenEnds))]
        public void ShouldHasAValorizedArtifactDirectoryName(string projectName)
        {
            var project = this.fixture.ProvideProject(this.output, projectName);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Theory]
        [MemberData(nameof(UnitTestProjectWhenEnds))]
        public void ShouldBeATestProject(string projectName)
        {
            var project = this.fixture.ProvideProject(this.output, projectName);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Theory]
        [MemberData(nameof(UnitTestProjectWhenEnds))]
        public void ShouldBeAUnitTestProject(string projectName)
        {
            var project = this.fixture.ProvideProject(this.output, projectName);

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Theory]
        [MemberData(nameof(UnitTestProjectWhenEnds))]
        public void ShouldNotBeAnIntegrationTestProject(string projectName)
        {
            var project = this.fixture.ProvideProject(this.output, projectName);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Theory]
        [MemberData(nameof(UnitTestProjectWhenEnds))]
        public void ShouldNotBeAPerformancenTestProject(string projectName)
        {
            var project = this.fixture.ProvideProject(this.output, projectName);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }
    }
}
