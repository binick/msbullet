// See the LICENSE.TXT file in the project root for full license information.

using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class PerformanceTestsProjectDefaultsTests : TestsProjectDefaultTests
    {
        public PerformanceTestsProjectDefaultsTests(ITestOutputHelper output, TestProjectFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact(DisplayName = "Should be an performance test project when the project name end with 'PerformanceTests'")]
        public void ShouldBeAPerformanceTestProjectWhenProjectNameEndWithPerformanceTests()
        {
            var project = this.ProvideProject("ClassLib1.PerformanceTests");

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(true);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }
    }
}
