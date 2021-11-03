using System.IO;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class PerformanceTestProjectDefaultsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public PerformanceTestProjectDefaultsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void ShouldHasAValorizedArtifactDirectoryName()
        {
            var project = this.fixture.ProvidePerformanceTestProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Fact]
        public void ShouldBeATestProject()
        {
            var project = this.fixture.ProvidePerformanceTestProject(this.output);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact]
        public void ShouldNotBeAUnitTestProject()
        {
            var project = this.fixture.ProvidePerformanceTestProject(this.output);

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldNotBeAnIntegrationTestProject()
        {
            var project = this.fixture.ProvidePerformanceTestProject(this.output);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldBeAPerformancenTestProject()
        {
            var project = this.fixture.ProvidePerformanceTestProject(this.output);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }
    }
}
