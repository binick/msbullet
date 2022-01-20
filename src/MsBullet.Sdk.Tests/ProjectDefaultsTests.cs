// See the LICENSE.TXT file in the project root for full license information.

using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public static IEnumerable<object[]> TargetFrameworksWithoutShippedRoslynAnalizersShipped => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => !(t.StartsWith("net5", StringComparison.OrdinalIgnoreCase) || t.StartsWith("net6", StringComparison.OrdinalIgnoreCase)))
            .Select(t => new object[] { t });

        public static IEnumerable<object[]> AllNet5TargetFrameworks => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => t.StartsWith("net5", StringComparison.OrdinalIgnoreCase))
            .Select(t => new object[] { t });

        public static IEnumerable<object[]> AllNet6TargetFrameworks => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => t.StartsWith("net6", StringComparison.OrdinalIgnoreCase))
            .Select(t => new object[] { t });

        public static IEnumerable<string> SupportedTargetFrameworks => new[]
        {
            "netcoreapp1.0",
            "netcoreapp1.1",
            "netcoreapp2.0",
            "netcoreapp2.1",
            "netcoreapp2.2",
            "netcoreapp3.0",
            "netcoreapp3.1",
            "net5.0",
            "net5.0-windows",
            "net6.0",
            "net6.0-android",
            "net6.0-ios",
            "net6.0-macos",
            "net6.0-maccatalyst",
            "net6.0-tvos",
            "net6.0-windows",
            "netstandard1.0",
            "netstandard1.1",
            "netstandard1.2",
            "netstandard1.3",
            "netstandard1.4",
            "netstandard1.5",
            "netstandard1.6",
            "netstandard2.0",
            "netstandard2.1",
            "net11",
            "net20",
            "net35",
            "net40",
            "net403",
            "net45",
            "net451",
            "net452",
            "net46",
            "net461",
            "net462",
            "net47",
            "net471",
            "net472",
            "net48",
            "netcore",
            "netcore45",
            "netcore45",
            "win",
            "win8",
            "netcore451",
            "win81",
            "netmf",
            "sl4",
            "sl5",
            "wp",
            "wp7",
            "wp75",
            "wp8",
            "wp81",
            "wpa81",
            "uap",
            "uap10.0",
            "win10",
            "netcore50"
        };

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

        [Fact(DisplayName = "Should have only implicit references to defined packages")]
        public void ShouldHaveOnlyImplicitlyDefinedPackageReference()
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

        [Fact(DisplayName = "Should have only reference packages with private assets")]
        public void ShouldHaveOnlyPackagesReferenceWithPrivateAssets()
        {
            var project = this.ProvideProject();

            project.ShouldContainItem("PackageReference")
                .Should()
                .SatisfyRespectively(i => i
                    .ShouldContainMetadata("PrivateAssets")
                    .ShouldEvaluatedEquivalentTo("all"));
        }

        [Theory(DisplayName = "Should be reference Roslyn analyzers when target framework is: ")]
        [MemberData(nameof(TargetFrameworksWithoutShippedRoslynAnalizersShipped))]
        public void ShouldBeReferenceRoslynAnalyzer(string targetFramework)
        {
            var project = this.ProvideProject(new Dictionary<string, string>
            {
                { "TargetFramework", targetFramework }
            });

            Func<ProjectItem, string, bool> ensureInclude = (i, packageName) => i.EvaluatedInclude.Equals(packageName, StringComparison.OrdinalIgnoreCase);

            project
                .ShouldContainItem("PackageReference")
                .Should()
                .Contain(i => ensureInclude(i, "Microsoft.VisualStudio.Threading.Analyzers"))
                .And
                .Contain(i => ensureInclude(i, "Microsoft.CodeAnalysis.NetAnalyzers"));
        }

        [Theory(DisplayName = "Should enforching all analysis rules")]
        [MemberData(nameof(AllNet5TargetFrameworks))]
        [MemberData(nameof(AllNet6TargetFrameworks))]
        public void ShouldEnforchingAllAnalysisRules(string targetFramework)
        {
            var expectedAnalysisMode = ProjectDefaultsTests.AllNet5TargetFrameworks.SelectMany(p => p.Cast<string>()).Contains(targetFramework)
                ? "AllEnabledByDefault"
                : "All";

            var project = this.ProvideProject(new Dictionary<string, string>
            {
                { "TargetFramework", targetFramework }
            });

            project
                .ShouldCountainProperty("AnalysisMode")
                .ShouldEvaluatedEquivalentTo(expectedAnalysisMode);

            project
                .ShouldCountainProperty("EnforceCodeStyleInBuild")
                .ShouldEvaluatedEquivalentTo(true);
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
            var project = this.ProvideProject(new Dictionary<string, string>
            {
                { packageVersionProperty, expectedVersion }
            });

            project.ShouldCountainProperty(packageVersionProperty).UnevaluatedValue
                .Should()
                .BeEquivalentTo(expectedVersion);
        }

        protected virtual Project ProvideProject(IDictionary<string, string> globalProperties = null)
        {
            var properties = new Dictionary<string, string>();

            if (this is UnitTestsProjectDefaultTests)
            {
                properties.Add("IsUnitTestProject", true.ToString(CultureInfo.InvariantCulture));
            }
            else if (this is IntegrationTestsProjectDefaultTests)
            {
                properties.Add("IsIntegrationTestProject", true.ToString(CultureInfo.InvariantCulture));
            }
            else if (this is PerformanceTestsProjectDefaultsTests)
            {
                properties.Add("IsPerformanceTestProject", true.ToString(CultureInfo.InvariantCulture));
            }

            return this.fixture.ProvideProject(this.output, globalProperties);
        }
    }
}
