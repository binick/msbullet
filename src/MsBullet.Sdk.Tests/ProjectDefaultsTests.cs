using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public static TheoryData<IDictionary<string, string>, IDictionary<string, bool>> ShouldIsATestProjectWhen => new TheoryData<IDictionary<string, string>, IDictionary<string, bool>>
        {
            {
                new Dictionary<string, string>
                {
                    { "IsUnitTestProject", "true" }
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", true },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", false },
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsIntegrationTestProject", "true" }
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", false },
                    { "IsIntegrationTestProject", true },
                    { "IsPerformanceTestProject", false },
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsUnitTestProject", "true" }
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", true },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", false },
                }
            },
            {
                new Dictionary<string, string>
                {
                    { "IsPerformanceTestProject", "true" }
                },
                new Dictionary<string, bool>
                {
                    { "IsTestProject", true },
                    { "IsUnitTestProject", false },
                    { "IsIntegrationTestProject", false },
                    { "IsPerformanceTestProject", true },
                }
            }
        };

        [Theory]
        [MemberData(nameof(ShouldIsATestProjectWhen))]
        public void ShouldBeATestProjectWhenSet(IDictionary<string, string> globalProperties, IDictionary<string, bool> expectedProperties)
        {
            if (globalProperties is null)
            {
                throw new ArgumentNullException(nameof(globalProperties));
            }

            if (expectedProperties is null)
            {
                throw new ArgumentNullException(nameof(expectedProperties));
            }

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

            foreach (var item in project
                .ShouldContainItem("Service"))
            {
                item
                    .ShouldEvaluatedEquivalentTo("{82a7f48d-3b50-4b1e-b82e-3ada8210c358}");
            }
        }
    }
}
