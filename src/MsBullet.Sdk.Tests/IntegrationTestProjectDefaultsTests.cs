// See the LICENSE.TXT file in the project root for full license information.

using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class IntegrationTestProjectDefaultsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public IntegrationTestProjectDefaultsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void ShouldHasAValorizedArtifactDirectoryName()
        {
            var project = this.fixture.ProvideIntegrationTestProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Fact]
        public void ShouldBeATestProject()
        {
            var project = this.fixture.ProvideIntegrationTestProject(this.output);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact]
        public void ShouldNotBeAUnitTestProject()
        {
            var project = this.fixture.ProvideIntegrationTestProject(this.output);

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldBeAnIntegrationTestProject()
        {
            var project = this.fixture.ProvideIntegrationTestProject(this.output);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact]
        public void ShouldNotBeAPerformancenTestProject()
        {
            var project = this.fixture.ProvideIntegrationTestProject(this.output);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }
    }
}
