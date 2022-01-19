// See the LICENSE.TXT file in the project root for full license information.

using Xunit;

namespace MsBullet.Sdk.Tests
{
    [CollectionDefinition(Name)]
    public class TestProjectCollection : ICollectionFixture<TestProjectFixture>
    {
        public const string Name = nameof(TestProjectCollection);
    }
}
