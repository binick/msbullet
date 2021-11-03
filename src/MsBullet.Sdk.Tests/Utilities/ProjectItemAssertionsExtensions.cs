using System;
using System.Globalization;
using System.IO;
using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectItemAssertionsExtensions
    {
        public static ProjectItem ShouldEvaluatedEquivalentTo<T>(this ProjectItem item, T value, string because = "", params object[] becauseArgs)
        {
            InternalShouldEvaluatedEquivalentTo(item.EvaluatedInclude, value, because, becauseArgs);

            return item;
        }

        private static void InternalShouldEvaluatedEquivalentTo<T>(string value, T expectedValue, string because = "", params object[] becauseArgs)
        {
            value
                .Should()
                .NotBeNull(because, becauseArgs);

            Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                .Should()
                .BeEquivalentTo(expectedValue, because, becauseArgs);
        }
    }
}
