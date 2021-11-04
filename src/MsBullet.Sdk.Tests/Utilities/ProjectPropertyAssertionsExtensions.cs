// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectPropertyAssertionsExtensions
    {
        public static ProjectProperty ShouldEvaluatedEquivalentTo<T>(this ProjectProperty property, T value, string because = "", params object[] becauseArgs)
        {
            InternalShouldEvaluatedEquivalentTo(property.EvaluatedValue, value, because, becauseArgs);

            return property;
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
