// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests
{
    [Collection(TestProjectCollection.Name)]
    public class MinimalRepoTests
    {
        private readonly ITestOutputHelper output;
        private readonly TestProjectFixture fixture;

        public MinimalRepoTests(ITestOutputHelper output, TestProjectFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void MinimalRepoBuildsWithoutErrors()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope.
            TestApp app = this.fixture.ProvideTestApp("MinimalRepo").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(this.output);

            // Then
            Assert.Equal(0, exitCode);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        public void MinimalRepoPackWithoutErrors(bool isShippable, bool shouldBeFoundInShippable, bool shouldBeFoundInNonShippable)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepo").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-pack",
                "/p:IsPackable=true",
                $"/p:IsShippable={isShippable}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", "NonShippable", "ClassLib1.0.1.0-dev.nupkg")).Equals(shouldBeFoundInNonShippable));
            Assert.True(File.Exists(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", "Shippable", "ClassLib1.0.1.0-dev.nupkg")).Equals(shouldBeFoundInShippable));
        }

        [Fact]
        public void MinimalRepoSignsWithoutErrors()
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            this.output.WriteLine("This feature will be released on future version");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            return;

#pragma warning disable CS0162 // Unreachable code detected

            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepo").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-sign",
                string.Format(CultureInfo.InvariantCulture, "-projects .{0}src{0}ClassLib1{0}ClassLib1.csproj", Path.DirectorySeparatorChar));

            // Then
            Assert.Equal(0, exitCode);

            var domain = AppDomain.CreateDomain(nameof(this.MinimalRepoSignsWithoutErrors));
            domain.Load(File.ReadAllBytes(Path.Combine(app.WorkingDirectory, "artifacts", "bin", "ClassLib1", "Debug", "netstandard2.1", "ClassLib1.dll")));
            Assert.NotEmpty(domain.GetAssemblies()[0].GetName().GetPublicKeyToken());
            AppDomain.Unload(domain);
#pragma warning restore CS0162 // Unreachable code detected
        }

        [Fact]
        public void MinimalRepoVersionWithoutErrors()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepo").Create(this.output);
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "/p:IsPackable=true",
                "/p:IsShippable=false");

            // Then
            Assert.Equal(0, exitCode);

            var version = AssemblyLoadContext.GetAssemblyName(Path.Combine(app.WorkingDirectory, "artifacts", "bin", "ClassLib1", "Debug", "netstandard2.1", "ClassLib1.dll")).Version;
            Assert.Equal("0.1.1", $"{version.Major}.{version.Minor}.{version.Minor}");
        }
    }
}
