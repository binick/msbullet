// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Xunit;
using Xunit.Abstractions;

namespace Flashy.Sdk.IntegrationTests
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
            TestApp app = this.fixture.CreateTestApp("MinimalRepo");
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(this.output);

            // Then
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void MinimalRepoPackNonShippableWithoutError()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepo");
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-pack" : "--pack",
                "/p:IsPackable=true",
                "/p:IsShippable=false");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", "NonShippable", "ClassLib1.1.0.0.nupkg")));
        }

        [Fact]
        public void MinimalRepoPackShippableWithoutError()
        {
            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepo");
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-pack" : "--pack",
                "/p:IsPackable=true",
                "/p:IsShippable=true");

            // Then
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(Path.Combine(app.WorkingDirectory, "artifacts", "packages", "Debug", "Shippable", "ClassLib1.1.0.0.nupkg")));
        }

        [Fact]
        public void MinimalRepoSignsWithoutError()
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            this.output.WriteLine("This feature will be released on future version");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            return;

#pragma warning disable CS0162 // Unreachable code detected

            // Given
#pragma warning disable CA2000 // Dispose objects before losing scope
            TestApp app = this.fixture.CreateTestApp("MinimalRepo");
#pragma warning restore CA2000 // Dispose objects before losing scope

            // When
            int exitCode = app.ExecuteBuild(
                this.output,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-sign" : "--sign",
                string.Format(CultureInfo.InvariantCulture, "-projects .{0}src{0}ClassLib1{0}ClassLib1.csproj", Path.DirectorySeparatorChar));

            // Then
            Assert.Equal(0, exitCode);

            var domain = AppDomain.CreateDomain(nameof(this.MinimalRepoSignsWithoutError));
            domain.Load(File.ReadAllBytes(Path.Combine(app.WorkingDirectory, "artifacts", "bin", "ClassLib1", "Debug", "netstandard2.1", "ClassLib1.dll")));
            Assert.NotEmpty(domain.GetAssemblies()[0].GetName().GetPublicKeyToken());
            AppDomain.Unload(domain);
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
