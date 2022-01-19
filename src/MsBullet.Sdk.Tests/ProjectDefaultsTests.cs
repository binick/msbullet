// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class ProjectDefaultsTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public ProjectDefaultsTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        public static TheoryData<IDictionary<string, string>> ProjectTypeDiscriminatorByProperties => new TheoryData<IDictionary<string, string>>
        {
            {
                new Dictionary<string, string>
                {
                    { "IsUnitTestProject", "false" },
                    { "IsIntegrationTestProject", "false" },
                    { "IsPerformanceTestProject", "false" }
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsUnitTestProject", "true" }
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsIntegrationTestProject", "true" }
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsPerformanceTestProject", "true" }
                }
            }
        };

        public static TheoryData<IDictionary<string, string>, IDictionary<string, bool>> TestProjectExpectedWhenHasProperties => InternalTestProjectExpectedWhenHasProperties();

        public static TheoryData<IDictionary<string, string>, IDictionary<string, string>> PackageReferenceVersionExpectedFor => InternalPackageReferenceVersionExpectedFor();

        [Theory]
        [MemberData(nameof(TestProjectExpectedWhenHasProperties))]
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Test data must be valorized.")]
        public void ShouldBeATestProjectWhenSet(IDictionary<string, string> globalProperties, IDictionary<string, bool> expectedProperties)
        {
            var project = this.fixture.ProvideProject(this.output, globalProperties);

            using (new AssertionScope())
            {
                foreach (var property in expectedProperties)
                {
                    project
                        .ShouldCountainProperty(property.Key)
                        .ShouldEvaluatedEquivalentTo(property.Value);
                }
            }
        }

        [Fact]
        public void ShouldHasAValorizedArtifactDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Fact]
        public void ShouldBeAValorizedArtifactDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("ArtifactsDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("artifacts");
        }

        [Theory]
        [InlineData("AnyCPU", "Debug", "Debug")]
        [InlineData("AnyCPU", "Release", "Release")]
        [InlineData("x86", "Debug", "x86", "Debug")]
        [InlineData("x86", "Release", "x86", "Release")]
        [InlineData("x64", "Debug", "x64", "Debug")]
        [InlineData("x64", "Release", "x64", "Release")]
        public void ShouldPlaceOutputBinariesIntoArtifactsBinaryDirectoryName(string platform, string configuration, params string[] expectedPathParts)
        {
            var project = this.fixture.ProvideProject(this.output, "MultiTargets", new Dictionary<string, string>
            {
                { "Platform", platform },
                { "Configuration", configuration }
            });

            project.ShouldCountainProperty("OutputPath").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Except(project.ShouldCountainProperty("BaseOutputPath").EvaluatedValue
                    .Trim(Path.DirectorySeparatorChar)
                    .Split(Path.DirectorySeparatorChar))
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
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Except(project.ShouldCountainProperty("BaseIntermediateOutputPath").EvaluatedValue
                    .Trim(Path.DirectorySeparatorChar)
                    .Split(Path.DirectorySeparatorChar))
                .Should()
                .ContainInOrder(expectedPathParts);
        }

        [Fact]
        public void ShouldNotBeATestProjectWhenProjectNameDoesNotEndWithTests()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldNotBeAUnitTestProjectWhenProjectNameDoesNotEndWithTestsOrUnitTests()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldNotBeAnIntegrationTestProjectWhenProjectNameDoesNotEndWithIntegrationTests()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldNotBeAPerformancenTestProjectWhenProjectNameDoesNotEndWithPerformanceTests()
        {
            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);
        }

        [Fact]
        public void ShouldMarkWithTestExplorerServiceTagWhenIsTestProject()
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { "IsTestProject", "true" }
            });

            foreach (var item in project.ShouldContainItem("Service"))
            {
                item.ShouldEvaluatedEquivalentTo("{82a7f48d-3b50-4b1e-b82e-3ada8210c358}");
            }
        }

        [Fact]
        public void ShouldHasAValorizedEngeeneringDirectoryName()
        {
            var project = this.fixture.ProvideProject(this.output);

            project.ShouldCountainProperty("RepositoryEngineeringDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("eng");
        }

        [Fact]
        public void ShouldUseStyleCopConfigurationFileFromEngineeringDirectory()
        {
            var project = this.fixture.ProvideProject(this.output);

            var repoEngDirProperty = new DirectoryInfo(project.ShouldCountainProperty("RepositoryEngineeringDir").EvaluatedValue);

            var styleCopConfigProperty = new FileInfo(project.ShouldCountainProperty("StyleCopConfig").EvaluatedValue);

            styleCopConfigProperty.Directory.FullName
                .Trim(Path.DirectorySeparatorChar)
                .Should()
                .BeEquivalentTo(repoEngDirProperty.FullName.Trim(Path.DirectorySeparatorChar));

            styleCopConfigProperty.Name
                .Should()
                .BeEquivalentTo("stylecop.json");
        }

        [Fact]
        public void ShouldRespectStyleCopConfigProperty()
        {
            var properties = new Dictionary<string, string>
            {
                { "StyleCopConfig", @"$(RepoRoot)\stylecop.json" }
            };

            var project = this.fixture.ProvideProject(this.output, properties);

            project.ShouldCountainProperty("StyleCopConfig").UnevaluatedValue
                .Should()
                .BeEquivalentTo(properties["StyleCopConfig"]);
        }

        [Theory]
        [MemberData(nameof(PackageReferenceVersionExpectedFor))]
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Test data must be valorized.")]
        public void ShouldHasOnlyPackageReferenceWithHighestAcceptableStableVersion(IDictionary<string, string> globalProperties, [NotNull] IDictionary<string, string> expectedPackageVersions)
        {
            var project = this.fixture.ProvideProject(this.output, globalProperties);

            using (new AssertionScope())
            {
                var items = project.ShouldContainItem("PackageReference");

                foreach (var item in items.ExceptBy(expectedPackageVersions.Select(p => p.Key), i => i.EvaluatedInclude))
                {
                    item.ShouldContainMetadata("Version").EvaluatedValue
                        .Should()
                        .MatchRegex(@"\d*.[*]");
                }

                foreach (var item in items.IntersectBy(expectedPackageVersions.Select(p => p.Key), i => i.EvaluatedInclude))
                {
                    item.ShouldContainMetadata("Version").EvaluatedValue
                        .Should()
                        .Be(expectedPackageVersions[item.EvaluatedInclude]);
                }
            }
        }

        [Fact]
        public void ShouldHasOnlyImplicitlyDefinedPackageReference()
        {
            var project = this.fixture.ProvideProject(this.output);

            using (new AssertionScope())
            {
                foreach (var item in project.ShouldContainItem("PackageReference"))
                {
                    item.ShouldContainMetadata("IsImplicitlyDefined")
                        .ShouldEvaluatedEquivalentTo(true);
                }
            }
        }

        [Fact]
        public void ShouldHasOnlyPackageReferenceWithPrivateAssets()
        {
            var project = this.fixture.ProvideProject(this.output);

            using (new AssertionScope())
            {
                foreach (var item in project.ShouldContainItem("PackageReference"))
                {
                    item.ShouldContainMetadata("PrivateAssets")
                        .ShouldEvaluatedEquivalentTo("all");
                }
            }
        }

        [Theory]
        [InlineData("MicrosoftCodeAnalysisFxCopAnalyzersVersion", "1.0.0")]
        [InlineData("MicrosoftVisualStudioThreadingAnalyzersVersion", "1.0.0")]
        [InlineData("StyleCopAnalyzersVersion", "1.0.0")]
        [InlineData("MicrosoftNETTestSdkVersion", "1.0.0")]
        [InlineData("XUnitAssertVersion", "1.0.0")]
        [InlineData("XUnitRunnerVisualstudioVersion", "1.0.0")]
        [InlineData("XUnitRunnerConsoleVersion", "1.0.0")]
        [InlineData("XUnitAbstractionsVersion", "1.0.0")]
        [InlineData("NerdbankGitVersioningVersion", "1.0.0")]
        public void ShouldRespectPackageReferenceVersion(string packageVersionProperty, string expectedVersion)
        {
            var project = this.fixture.ProvideProject(this.output, new Dictionary<string, string>
            {
                { packageVersionProperty, expectedVersion }
            });

            project.ShouldCountainProperty(packageVersionProperty).UnevaluatedValue
                .Should()
                .BeEquivalentTo(expectedVersion);
        }

        private static TheoryData<IDictionary<string, string>, IDictionary<string, bool>> InternalTestProjectExpectedWhenHasProperties()
        {
            var expectedProperties = new Dictionary<string, bool>[]
            {
                new Dictionary<string, bool>
                {
                    { "IsTestProject", false },
                    { "IsUnitTestProject", false },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", false },
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", true },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", false },
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", false },
                    { "IsIntegrationTestProject", true },
                    { "IsPerformanceTestProject", false },
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", false },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", true },
                }
            };

            var set = new TheoryData<IDictionary<string, string>, IDictionary<string, bool>>();
            var counter = 0;
            foreach (var data in ProjectTypeDiscriminatorByProperties)
            {
                set.Add((IDictionary<string, string>)data[0], expectedProperties[counter++]);
            }

            return set;
        }

        private static TheoryData<IDictionary<string, string>, IDictionary<string, string>> InternalPackageReferenceVersionExpectedFor()
        {
            var expectedPackageVersions = new Dictionary<string, string>[]
            {
                new Dictionary<string, string>(),
                new Dictionary<string, string>
                {
                    { "xunit.runner.console", "2.4.1" },
                    { "xunit.runner.visualstudio", "2.4.3" }
                },
                new Dictionary<string, string>
                {
                    { "xunit.runner.console", "2.4.1" },
                    { "xunit.runner.visualstudio", "2.4.3" }
                },
                new Dictionary<string, string>
                {
                    { "xunit.runner.console", "2.4.1" },
                    { "xunit.runner.visualstudio", "2.4.3" }
                }
            };

            var set = new TheoryData<IDictionary<string, string>, IDictionary<string, string>>();
            var counter = 0;
            foreach (var data in ProjectTypeDiscriminatorByProperties)
            {
                set.Add((IDictionary<string, string>)data[0], expectedPackageVersions[counter++]);
            }

            return set;
        }
    }
}
