// See the LICENSE.TXT file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class SingleBuildOutputTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public SingleBuildOutputTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be provided with a default artifact directory name")]
        public void ShouldBeProvidedWithADefaultArtifactDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("artifacts");
        }

        [Fact(DisplayName = "Should be provided with a default intermediate object directory name")]
        public void ShouldBeProvidedWithADefaultIntermediateObjectDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("ArtifactsObjDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("obj")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Fact(DisplayName = "Should be provided with a default build output directory name")]
        public void ShouldBeProvidedWithADefaultBuildOutputDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("ArtifactsBinDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("bin")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default log directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultLogDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsLogDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName(Path.Join("log", configuration))
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default temporary files directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultTemporaryFilesDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsTmpDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName(Path.Join("tmp", configuration))
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default test results directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultTestResultsDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsTestResultsDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName(Path.Join("TestResults", configuration))
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default test report directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultReportDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsReportDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("Reports")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsTestResultsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default symbols store directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultCoverageDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsCoverageDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("Coverage")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsTestResultsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default symbols store directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultSymbolsStoreDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsSymStoreDirectory").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName(Path.Join("SymStore", configuration))
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default output packages directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultOutputPackagesDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsPackagesDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName(Path.Join("packages", configuration))
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default output shippable packages directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultShippablePackagesDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsShippingPackagesDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("Shippable")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsPackagesDir").EvaluatedValue);
        }

        [Theory(DisplayName = "Should be provided with a default output non shippable packages directory name")]
        [InlineData("Debug")]
        [InlineData("Release")]
        public void ShouldBeProvidedWithADefaultNonShippablePackagesDirectoryName(string configuration)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("ArtifactsNonShippingPackagesDir").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasFolderName("NonShippable")
                .And
                .Contain(project.ShouldCountainProperty("ArtifactsPackagesDir").EvaluatedValue);
        }
    }
}
