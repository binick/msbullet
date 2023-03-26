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

        [Fact(DisplayName = "Should be include StyleCop configuration file into project root")]
        public void ShouldBeIncludeStyleCopConfigIntoProjectRoot()
        {
            var project = this.fixture.ProvideProject(this.output);

            using (new AssertionScope())
            {
                var andWitchConstraint = project
                    .ShouldContainItem("AdditionalFiles")
                    .Should()
                    .ContainSingle(i => i.EvaluatedInclude.Equals(project.ShouldCountainProperty("StyleCopConfig", string.Empty).EvaluatedValue, StringComparison.OrdinalIgnoreCase));

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
                    .ContainSingle(c => c.Name.Equals("Visible", StringComparison.OrdinalIgnoreCase))
                    .Which
                    .UnevaluatedValue
                    .Should()
                    .Be("$(ShowStyleCopConfig)");
            }
        }

        [Fact(DisplayName = "Should not be include StyleCop configuration file into project root when it is opted out")]
        public void ShouldNotIncludeStyleCopConfigIntoProjectRootWhenItIsOptedOut()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "UsingToolStyleCopAnalyzers", "false" }
            });

            project
                .ShouldContainItem("AdditionalFiles")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals(project.ShouldCountainProperty("StyleCopConfig", string.Empty).EvaluatedValue, StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should be referenced StyleCopAnalyzers package")]
        public void ShouldBeReferencedStyleCopAnalyzersPackage()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .Contain(i => i.EvaluatedInclude.Equals("StyleCop.Analyzers", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "Should not be referenced StyleCopAnalyzers package when it is opted out")]
        public void ShouldNotBeReferencedStyleCopAnalyzersPackageWhenItIsOptedOut()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "UsingToolStyleCopAnalyzers", "false" }
            });

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .NotContain(i => i.EvaluatedInclude.Equals("StyleCop.Analyzers", StringComparison.OrdinalIgnoreCase));
        }
    }
}
