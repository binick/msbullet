// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Globalization;

namespace FluentAssertions
{
    internal static class ObjectAssertionsExtensions
    {
        public static void BeEquivalentTo<T>(this IConvertible value, T expectedValue, string because = "", params object[] becauseArgs)
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
