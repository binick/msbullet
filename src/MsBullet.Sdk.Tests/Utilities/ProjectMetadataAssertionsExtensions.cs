// See the LICENSE.TXT file in the project root for full license information.

using System;
using FluentAssertions;
using FluentAssertions.Equivalency;

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

        public static ProjectMetadata ShouldEvaluatedEquivalentTo<T>(this ProjectMetadata item, T value, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options = null, string because = "", params object[] becauseArgs)
        {
            item.EvaluatedValue
                .BeEquivalentTo(value, options, because, becauseArgs);

            return item;
        }

        public static ProjectMetadata ShouldUnevaluatedEquivalentTo<T>(this ProjectMetadata item, T value, string because = "", params object[] becauseArgs)
        {
            item.UnevaluatedValue
                .BeEquivalentTo(value, because, becauseArgs);

            return item;
        }

        public static ProjectMetadata ShouldUnevaluatedEquivalentTo<T>(this ProjectMetadata item, T value, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options = null, string because = "", params object[] becauseArgs)
        {
            item.UnevaluatedValue
                .BeEquivalentTo(value, options, because, becauseArgs);

            return item;
        }
    }
}
