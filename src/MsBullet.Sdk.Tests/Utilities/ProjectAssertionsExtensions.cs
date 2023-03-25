// See the LICENSE.TXT file in the project root for full license information.

using System.Collections.Generic;
using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectAssertionsExtensions
    {
        public static ProjectProperty ShouldCountainProperty(this Project project, string name, string because = "", params object[] becauseArgs)
        {
            return project.GetProperty(name)
                .Should()
                .NotBeNull(because, becauseArgs)
                .And
                .Subject
                .As<ProjectProperty>();
        }

        public static IEnumerable<ProjectItem> ShouldContainItem(this Project project, string itemType, string because = "", params object[] becauseArgs)
        {
            return project.GetItemsIgnoringCondition(itemType)
                .Should()
                .NotBeNull(because, becauseArgs)
                .And
                .Subject
                .As<IEnumerable<ProjectItem>>();
        }
    }
}
