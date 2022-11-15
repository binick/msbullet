// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static IEnumerable<object[]> TargetFrameworksWithoutShippedRoslynAnalizersShipped => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => !(t.Cast<string>().First().StartsWith("net5", StringComparison.OrdinalIgnoreCase) ||
                t.Cast<string>().First().StartsWith("net6", StringComparison.OrdinalIgnoreCase) ||
                t.Cast<string>().First().StartsWith("net7", StringComparison.OrdinalIgnoreCase)));

        public static IEnumerable<object[]> AllNet5TargetFrameworks => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => t.Cast<string>().First().StartsWith("net5", StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<object[]> AllNet6TargetFrameworks => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => t.Cast<string>().First().StartsWith("net6", StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<object[]> AllNet7TargetFrameworks => ProjectDefaultsTests.SupportedTargetFrameworks
            .Where(t => t.Cast<string>().First().StartsWith("net7", StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<object[]> SupportedTargetFrameworks => new object[][]
        {
            new string[] { "netcoreapp1.0" },
            new string[] { "netcoreapp1.1" },
            new string[] { "netcoreapp2.0" },
            new string[] { "netcoreapp2.1" },
            new string[] { "netcoreapp2.2" },
            new string[] { "netcoreapp3.0" },
            new string[] { "netcoreapp3.1" },
            new string[] { "net5.0" },
            new string[] { "net5.0-windows" },
            new string[] { "net6.0" },
            new string[] { "net6.0-android" },
            new string[] { "net6.0-ios" },
            new string[] { "net6.0-macos" },
            new string[] { "net6.0-maccatalyst" },
            new string[] { "net6.0-tvos" },
            new string[] { "net6.0-windows" },
            new string[] { "net7.0" },
            new string[] { "net7.0-android" },
            new string[] { "net7.0-ios" },
            new string[] { "net7.0-macos" },
            new string[] { "net7.0-maccatalyst" },
            new string[] { "net7.0-tvos" },
            new string[] { "net7.0-windows" },
            new string[] { "netstandard1.0" },
            new string[] { "netstandard1.1" },
            new string[] { "netstandard1.2" },
            new string[] { "netstandard1.3" },
            new string[] { "netstandard1.4" },
            new string[] { "netstandard1.5" },
            new string[] { "netstandard1.6" },
            new string[] { "netstandard2.0" },
            new string[] { "netstandard2.1" },
            new string[] { "net11" },
            new string[] { "net20" },
            new string[] { "net35" },
            new string[] { "net40" },
            new string[] { "net403" },
            new string[] { "net45" },
            new string[] { "net451" },
            new string[] { "net452" },
            new string[] { "net46" },
            new string[] { "net461" },
            new string[] { "net462" },
            new string[] { "net47" },
            new string[] { "net471" },
            new string[] { "net472" },
            new string[] { "net48" },
            new string[] { "netcore" },
            new string[] { "netcore45" },
            new string[] { "netcore451" },
            new string[] { "netmf" },
            new string[] { "sl4" },
            new string[] { "sl5" },
            new string[] { "wp" },
            new string[] { "wp7" },
            new string[] { "wp75" },
            new string[] { "wp8" },
            new string[] { "wp81" },
            new string[] { "wpa81" },
            new string[] { "uap" },
            new string[] { "uap10.0" }
        };

        public IEnumerable<string> FrameworkDependentPackages => new string[]
        {
            "Microsoft.NETCore.App",
            "NETStandard.Library"
        };

        [Fact(DisplayName = "Should has a valorized engeenering directory name")]
        public virtual void ShouldHasAValorizedEngeeneringDirectoryName()
        {
            var project = this.ProvideProject();

            project.ShouldCountainProperty("RepositoryEngineeringDir").EvaluatedValue
                .Trim(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .Should()
                .EndWith("eng");
        }

        [Fact(DisplayName = "Should have only implicit references to defined packages")]
        public virtual void ShouldHaveOnlyImplicitlyDefinedPackageReference()
        {
            var project = this.ProvideProject();

            using (new AssertionScope())
            {
                foreach (var item in project.ShouldContainItem("PackageReference"))
                {
                    item.ShouldContainMetadata("IsImplicitlyDefined")
                        .ShouldEvaluatedEquivalentTo(true, options => options);
                }
            }
        }

        [Theory(DisplayName = "Should have only reference packages with private assets")]
        [MemberData(nameof(SupportedTargetFrameworks))]
        public void ShouldHaveOnlyPackagesReferenceWithPrivateAssets(string targetFramework)
        {
            var project = this.ProvideProject(new Dictionary<string, string>
            {
                { "TargetFramework", targetFramework }
            });

            var excludePackages = this.FrameworkDependentPackages;

#pragma warning disable S1135 // Track uses of "TODO" tags
            /*
             * Todo: Microsoft.NET.Test.Sdk should not be there.
             * Investigate why it is added even though the IsTestProject property is false.
             *
             * Todo: System.Runtime.InteropServices.NFloat.Internal
             * This package reference is shipped whitin a iOS and macOS runtimes.
             */
#pragma warning restore S1135 // Track uses of "TODO" tags
            bool.TryParse(project.GetPropertyValue("IsTestProject"), out var isTestProject);
            if (!isTestProject)
            {
                excludePackages = excludePackages.Append("Microsoft.NET.Test.Sdk");
                excludePackages = excludePackages.Append("System.Runtime.InteropServices.NFloat.Internal");
            }

            using (new AssertionScope())
            {
                foreach (var item in project.ShouldContainItem("PackageReference").ExceptBy(excludePackages, i => i.EvaluatedInclude))
                {
                    item
                        .ShouldContainMetadata("PrivateAssets")
                        .ShouldEvaluatedEquivalentTo("All", options => options.Using<string>(ctx => ctx.Subject.ToUpperInvariant().Should().Be(ctx.Expectation.ToUpperInvariant())).WhenTypeIs<string>());
                }
            }
        }

        [Theory(DisplayName = "Should be reference Roslyn analyzers when target framework is: ")]
        [MemberData(nameof(SupportedTargetFrameworks))]
        public virtual void ShouldBeReferenceRoslynAnalyzer(string targetFramework)
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
        [MemberData(nameof(AllNet7TargetFrameworks))]
        public virtual void ShouldEnforchingAllAnalysisRules(string targetFramework)
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

        [Fact(DisplayName = "Should respect user package reference verions")]
        public virtual void ShouldRespectUserPackageReferenceVersions()
        {
            var project = this.ProvideProject();

            var excludePackages = this.FrameworkDependentPackages;

#pragma warning disable S1135 // Track uses of "TODO" tags
            /*
             * Todo: Microsoft.NET.Test.Sdk should not be there.
             * Investigate why it is added even though the IsTestProject property is false.
             */
#pragma warning restore S1135 // Track uses of "TODO" tags
            bool.TryParse(project.GetPropertyValue("IsTestProject"), out var isTestProject);
            if (!isTestProject)
            {
                excludePackages = excludePackages.Append("Microsoft.NET.Test.Sdk");
            }

            using (new AssertionScope())
            {
                foreach (var item in project.ShouldContainItem("PackageReference").ExceptBy(excludePackages, i => i.EvaluatedInclude))
                {
                    var propertyName = new string(item.ShouldContainMetadata("Version").UnevaluatedValue.Skip(2).SkipLast(1).ToArray());
                    project
                        .ShouldCountainProperty(propertyName).Xml.Condition
                        .Should()
                        .Be($"'$({propertyName})' == ''");
                }
            }
        }

        [Fact(DisplayName = "Should be packable")]
        public void ShouldBePackable()
        {
            var project = this.ProvideProject();

            project
                .ShouldCountainProperty("IsPackable")
                .ShouldEvaluatedEquivalentTo(true);
        }

        protected virtual Project ProvideProject(IDictionary<string, string> globalProperties = null)
        {
            globalProperties ??= new Dictionary<string, string>();

            if (this is UnitTestsProjectDefaultTests)
            {
                globalProperties.Add("IsUnitTestProject", true.ToString(CultureInfo.InvariantCulture));
            }
            else if (this is IntegrationTestsProjectDefaultTests)
            {
                globalProperties.Add("IsIntegrationTestProject", true.ToString(CultureInfo.InvariantCulture));
            }
            else if (this is PerformanceTestsProjectDefaultsTests)
            {
                globalProperties.Add("IsPerformanceTestProject", true.ToString(CultureInfo.InvariantCulture));
            }

            return this.fixture.ProvideProject(this.output, globalProperties);
        }
    }
}
