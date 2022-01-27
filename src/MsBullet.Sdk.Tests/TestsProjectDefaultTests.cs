// See the LICENSE.TXT file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    public abstract class TestsProjectDefaultTests : ProjectDefaultsTests
    {
        protected TestsProjectDefaultTests(ITestOutputHelper output, TestProjectFixture fixture)
            : base(output, fixture)
        {
            this.Output = output;
            this.Fixture = fixture;
        }

        protected ITestOutputHelper Output { get; }

        protected TestProjectFixture Fixture { get; }

        [Fact(DisplayName = "Should be marked with the Test Explorer Service Tag when it is a test project")]
        public void ShouldBeMarkedWithTestExplorerServiceTagWhenIsTestProject()
        {
            var project = this.ProvideProject();

            foreach (var item in project.ShouldContainItem("Service"))
            {
                item.ShouldEvaluatedIncludeEquivalentTo("{82a7f48d-3b50-4b1e-b82e-3ada8210c358}");
            }
        }

        [Fact(DisplayName = "Should only have reference packages with the highest acceptable stable version")]
        public void ShouldOnlyHaveReferencePackagesWithTheHighestAcceptableStableVersion()
        {
            var project = this.ProvideProject();

            var expectedPackageVersions = new Dictionary<string, string>
            {
                { "xunit.runner.console", "2.4.1" },
                { "xunit.runner.visualstudio", "2.4.3" }
            };

            using (new AssertionScope())
            {
                var items = project.ShouldContainItem("PackageReference");

                foreach (var item in items.ExceptBy(expectedPackageVersions.Select(p => p.Key).Concat(this.FrameworkDependentPackages), i => i.EvaluatedInclude))
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

        [Fact(DisplayName = "Should not be packable")]
        public void ShouldNotBePackable()
        {
            var project = this.ProvideProject();

            project
                .ShouldCountainProperty("IsPackable")
                .ShouldEvaluatedEquivalentTo(false);
        }

        protected virtual Project ProvideProject(string projectName, IDictionary<string, string> globalProperties = null)
        {
            return this.Fixture.ProvideProject(this.Output, projectName, globalProperties);
        }
    }
}
