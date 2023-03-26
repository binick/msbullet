// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Execution;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectTargetAssertionsExtensions
    {
        public static KeyValuePair<string, ProjectTargetInstance> ShouldCountainSingleTarget(this Project project, string name, string because = "", params object[] becauseArgs)
        {
            return project.Targets
                .Should()
                .ContainSingle(i => i.Key.Equals(name, StringComparison.OrdinalIgnoreCase), because, becauseArgs)
                .Subject;
        }
    }
}
