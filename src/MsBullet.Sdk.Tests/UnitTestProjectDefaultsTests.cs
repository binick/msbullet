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

        [Fact]
        public void ShouldHasAValorizedArtifactDirectoryName()
        {
            var project = this.fixture.ProvideUnitTestProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Fact]
        public void ShouldBeATestProject()
        {
            var project = this.fixture.ProvideUnitTestProject(this.output);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact]
        public void ShouldBeAUnitTestProject()
        {
            var project = this.fixture.ProvideUnitTestProject(this.output);

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact]
        public void ShouldNotBeAnIntegrationTestProject()
        {
            var project = this.fixture.ProvideUnitTestProject(this.output);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldNotBeAPerformancenTestProject()
        {
            var project = this.fixture.ProvideUnitTestProject(this.output);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }
    }
}
