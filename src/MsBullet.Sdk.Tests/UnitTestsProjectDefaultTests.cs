// See the LICENSE.TXT file in the project root for full license information.

using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class UnitTestsProjectDefaultTests : TestsProjectDefaultTests
    {
        public UnitTestsProjectDefaultTests(ITestOutputHelper output, TestProjectFixture fixture)
            : base(output, fixture)
        {
        }

        [Theory(DisplayName = "Should be an unit test project when the project name end with: ")]
        [InlineData("Tests")]
        [InlineData("UnitTests")]
        public void ShouldBeAnUnitTestProjectWhenProjectNameEndWith(string value)
        {
            var project = this.ProvideProject($"ClassLib1.{value}");

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(true);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }
    }
}
