// See the LICENSE.TXT file in the project root for full license information.

using System.IO;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace FluentAssertions
{
    internal static class PathAssertionExtensions
    {
        public static string ShouldBeAValidPath(this string value, string because = "", params object[] becauseArgs)
        {
            var act = () =>
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(value);
            };

            act
                .Should()
                .NotThrow(because, becauseArgs);

            return value;
        }

        public static AndConstraint<StringAssertions> ShouldHasFolderName(this string path, string expcected, string because = "", params object[] becauseArgs)
        {
            return path
                .Trim(Path.DirectorySeparatorChar)
                .Should()
                .EndWith(expcected, because, becauseArgs);
        }

        public static AndConstraint<StringAssertions> ShouldHasParent(this string path, string expcected, string because = "", params object[] becauseArgs)
        {
            return path
                .Replace('/', Path.DirectorySeparatorChar)
                .Should()
                .StartWith(expcected, because, becauseArgs);
        }
    }
}
