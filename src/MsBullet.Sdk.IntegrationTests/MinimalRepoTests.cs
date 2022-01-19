// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
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
        [InlineData(false, "NonShippable")]
        [InlineData(true, "Shippable")]
        public void MinimalRepoPackWithoutErrors(bool isShippable, string destinationFolder)
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.ProvideTestApp("MinimalRepo")
                .WithPreCreate("git", "init")
                .WithPreCreate("git", "remote", "add", "origin", "http://localhost")
                .WithPreCreate("git", "checkout", "-b", "main")
                .WithPreCreate("git", "commit", "--allow-empty", "-m", "Dummy happy empty commit.")
                .Create(this.output);

#pragma warning restore CA2000 // Dispose objects before losing scope
            var expectedVersion = "1.0.0-local.*";
            var packageFileName = $"ClassLib1.{expectedVersion}.nupkg";

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                "-pack",
                "/p:IsPackable=true",
                $"/p:IsShippable={isShippable}");

            // Then
            Assert.Equal(0, exitCode);
            Assert.Single(Directory.GetFiles(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", destinationFolder), packageFileName));
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

#pragma warning disable SYSLIB0024 // Type or member is obsolete
            var domain = AppDomain.CreateDomain(nameof(this.MinimalRepoSignsWithoutErrors));
#pragma warning restore SYSLIB0024 // Type or member is obsolete
            domain.Load(File.ReadAllBytes(Path.Combine(app.WorkingDirectory, "artifacts", "bin", "ClassLib1", "Debug", "netstandard2.0", "ClassLib1.dll")));
            Assert.NotEmpty(domain.GetAssemblies()[0].GetName().GetPublicKeyToken());
#pragma warning disable SYSLIB0024 // Type or member is obsolete
            AppDomain.Unload(domain);
#pragma warning restore SYSLIB0024 // Type or member is obsolete
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

            var version = AssemblyLoadContext.GetAssemblyName(Path.Combine(app.WorkingDirectory, "artifacts", "bin", "ClassLib1", "Debug", "netstandard2.0", "ClassLib1.dll")).Version;
            Assert.Equal("1.0.0", $"{version.Major}.{version.Minor}.{version.Minor}");
        }
    }
}
