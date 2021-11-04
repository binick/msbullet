// See the LICENSE.TXT file in the project root for full license information.

using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectMetadataAssertionsExtensions
    {
        public static ProjectMetadata ShouldEvaluatedEquivalentTo<T>(this ProjectMetadata item, T value, string because = "", params object[] becauseArgs)
        {
            item.EvaluatedValue
                .BeEquivalentTo(value, because, becauseArgs);

            return item;
        }
    }
}
