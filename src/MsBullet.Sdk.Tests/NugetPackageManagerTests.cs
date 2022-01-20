// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class NugetPackageManagerTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public NugetPackageManagerTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Should be provided with a default location when fallback property or environment variable is empty")]
        public void ShouldBeProvidedWithADefaultLocation()
        {
            var expected = $"^{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages").Replace($"{Path.DirectorySeparatorChar}", string.Format(CultureInfo.InvariantCulture, "{0}{0}", Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)}\\\\?$";

            var project = this.fixture.ProvideProject(this.output);

            project
                .ShouldCountainProperty("NuGetPackageRoot")
                .ShouldEvaluatedMatchRegex(expected)
                .EvaluatedValue
                .ShouldBeAValidPath();
        }

        [Theory(DisplayName = "Should be provided with a specified location when: ")]
        [InlineData(null, "OverridePackagesLocation")]
        [InlineData("FallbackPackagesLocation", null)]
        [InlineData("FallbackPackagesLocation", "OverridePackagesLocation")]
        public void ShouldBeProvidedWithASpecifiedLocation(string fallbackProperty, string environmentVariable)
        {
            var properties = new Dictionary<string, string>();
            if (fallbackProperty is not null)
            {
                properties.Add("NuGetPackageRoot", fallbackProperty);
            }

            if (environmentVariable is not null)
            {
                environmentVariable = Path.Combine(Path.GetTempPath(), environmentVariable);
                properties.Add("NUGET_PACKAGES", environmentVariable);
            }

            var expected = !properties.ContainsKey("NuGetPackageRoot")
                ? properties["NUGET_PACKAGES"]
                : properties["NuGetPackageRoot"];

            var project = this.fixture.ProvideProject(this.output, properties);

            project
                .ShouldCountainProperty("NuGetPackageRoot")
                .ShouldEvaluatedMatchRegex($"^{expected.Replace($"{Path.DirectorySeparatorChar}", string.Format(CultureInfo.InvariantCulture, "{0}{0}", Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)}\\\\?$")
                .EvaluatedValue
                .ShouldBeAValidPath();
        }
    }
}
