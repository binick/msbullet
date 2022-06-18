// See the LICENSE.TXT file in the project root for full license information.

using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class CodingStyleAndConsistencyToolsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public CodingStyleAndConsistencyToolsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be use StyleCop as default tool")]
        public void ShouldBeUseStyleCopAsDefaultTool()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("UsingToolStyleCopAnalyzers")
                .ShouldEvaluatedEquivalentTo(true);
        }

        [Fact(DisplayName = "Should be respect user defined StyleCop configuration file path")]
        public void ShouldBeRespectUserDefinedStyleCopConfigPath()
        {
            var properties = new Dictionary<string, string>
            {
                { "StyleCopConfig", @"$(RepoRoot)\stylecop.json" }
            };

            var project = this.fixture.ProvideProject(this.output, properties);

            project
                .ShouldCountainProperty("StyleCopConfig")
                .ShouldEvaluatedEquivalentTo(properties["StyleCopConfig"]);
        }

        [Fact(DisplayName = "Should be use StyleCop configuration file into engeenering directory")]
        public void ShouldBeUseStyleCopConfigIntoEngeeneringDirectory()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("StyleCopConfig")
                .EvaluatedValue
                .ShouldBeAValidPath()
                .ShouldHasParent(project.ShouldCountainProperty("RepositoryEngineeringDir").EvaluatedValue);
        }

        [Fact(DisplayName = "Should be place StyleCop configuration file into project root")]
        public void ShouldBePlaceStyleCopConfigIntoProjectRoot()
        {
            var project = this.fixture.ProvideProject(this.output);

            using (new AssertionScope())
            {
                var andWitchConstraint = project
                    .ShouldContainItem("AdditionalFiles")
                    .Should()
                    .ContainSingle(i => i.EvaluatedInclude.Equals(project.ShouldCountainProperty("StyleCopConfig", string.Empty).EvaluatedValue));

                andWitchConstraint
                    .Which
                    .ShouldContainMetadata("Link")
                    .ShouldEvaluatedEquivalentTo("stylecop.json", options => options);

                andWitchConstraint
                    .Which
                    .Xml
                    .Condition
                    .Should()
                    .Be("Exists('$(StyleCopConfig)')");

                andWitchConstraint
                    .Which
                    .DirectMetadata
                    .Should()
                    .ContainSingle(c => c.Name.Equals("Visible"))
                    .Which
                    .UnevaluatedValue
                    .Should()
                    .Be("$(ShowStyleCopConfig)");
            }
        }
    }
}
