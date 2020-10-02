// See the LICENSE.TXT file in the project root for full license information.

using MsBullet.Build.Tasks.Coverlet.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Build.Tasks.Coverlet.Tests
{
    [Collection(MsBuildTaskProviderCollection.Name)]
    public class CoverletInstrumentationTaskTests
    {
        private readonly ITestOutputHelper output;
        private readonly MsBuildTaskProviderFixture fixture;

        public CoverletInstrumentationTaskTests(ITestOutputHelper output, MsBuildTaskProviderFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void GenerateIstrumentationState()
        {
            // Arrange
            var task = MsBuildTaskProviderFixture.Setup<CoverletInstrumentationTask>(this.output);

            // Act
            var result = task.Execute();

            // Assert
            Assert.True(result);
        }
    }
}
