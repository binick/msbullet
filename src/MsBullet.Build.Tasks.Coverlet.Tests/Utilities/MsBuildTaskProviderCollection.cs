// See the LICENSE.TXT file in the project root for full license information.

using Xunit;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    [CollectionDefinition(Name)]
    public class MsBuildTaskProviderCollection : ICollectionFixture<MsBuildTaskProviderFixture>
    {
        public const string Name = nameof(MsBuildTaskProviderCollection);
    }
}
