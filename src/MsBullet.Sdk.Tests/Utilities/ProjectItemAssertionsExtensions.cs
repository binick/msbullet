// See the LICENSE.TXT file in the project root for full license information.

using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectItemAssertionsExtensions
    {
        public static ProjectItem ShouldEvaluatedEquivalentTo<T>(this ProjectItem item, T value, string because = "", params object[] becauseArgs)
        {
            item.EvaluatedInclude
                .BeEquivalentTo(value, because, becauseArgs);

            return item;
        }

        public static ProjectMetadata ShouldContainMetadata(this ProjectItem item, string name, string because = "", params object[] becauseArgs)
        {
            return item.GetMetadata(name)
                .Should()
                .NotBeNull(because, becauseArgs)
                .And
                .Subject
                .As<ProjectMetadata>();
        }
    }
}
