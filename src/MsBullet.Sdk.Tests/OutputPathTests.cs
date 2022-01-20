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
    public class OutputPathTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public OutputPathTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory(DisplayName = "Should place the output binaries in a directory named")]
        [InlineData("AnyCPU", "Debug", "Debug")]
        [InlineData("AnyCPU", "Release", "Release")]
        [InlineData("x86", "Debug", "x86", "Debug")]
        [InlineData("x86", "Release", "x86", "Release")]
        [InlineData("x64", "Debug", "x64", "Debug")]
        [InlineData("x64", "Release", "x64", "Release")]
        public void ShouldPlaceOutputBinariesInADirectoryNamed(string platform, string configuration, params string[] expectedPathParts)
        {
            var project = this.fixture.ProvideProject(this.output, "MultiTargets", new Dictionary<string, string>
            {
                { "Platform", platform },
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("OutputPath").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasParent(project.ShouldCountainProperty("BaseOutputPath").EvaluatedValue)
                .And
                .Subject
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .ContainInOrder(expectedPathParts);
        }

        [Theory]
        [InlineData("AnyCPU", "Debug", "Debug")]
        [InlineData("AnyCPU", "Release", "Release")]
        [InlineData("x86", "Debug", "x86", "Debug")]
        [InlineData("x86", "Release", "x86", "Release")]
        [InlineData("x64", "Debug", "x64", "Debug")]
        [InlineData("x64", "Release", "x64", "Release")]
        public void ShouldPlaceIntermediateLanguageIntoArtifactsBinaryDirectoryName(string platform, string configuration, params string[] expectedPathParts)
        {
            var project = this.fixture.ProvideProject(this.output, "MultiTargets", new Dictionary<string, string>
            {
                { "Platform", platform },
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("IntermediateOutputPath").EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasParent(project.ShouldCountainProperty("BaseIntermediateOutputPath").EvaluatedValue)
                .And
                .Subject
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .ContainInOrder(expectedPathParts);
        }
    }
}
