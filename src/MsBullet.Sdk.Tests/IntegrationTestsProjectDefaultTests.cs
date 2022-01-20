// See the LICENSE.TXT file in the project root for full license information.

using Microsoft.Build.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace MsBullet.Sdk.Tests
{
    [Collection(TestProjectCollection.Name)]
    public class IntegrationTestsProjectDefaultTests : TestsProjectDefaultTests
    {
        public IntegrationTestsProjectDefaultTests(ITestOutputHelper output, TestProjectFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact(DisplayName = "Should be an integration test project when the project name end with 'IntegrationTests'")]
        public void ShouldBeAnIntegrationTestProjectWhenProjectNameEndWithIntegrationTests()
        {
            var project = this.ProvideProject("ClassLib1.IntegrationTests");

            project
                .ShouldCountainProperty("IsUnitTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsIntegrationTestProject")
                .ShouldEvaluatedEquivalentTo(true);

            project
                .ShouldCountainProperty("IsPerformanceTestProject")
                .ShouldEvaluatedEquivalentTo(false);

            project
                .ShouldCountainProperty("IsTestProject")
                .ShouldEvaluatedEquivalentTo(true);
        }
    }
}
