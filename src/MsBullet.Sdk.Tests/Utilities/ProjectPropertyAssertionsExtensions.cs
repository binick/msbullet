// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectPropertyAssertionsExtensions
    {
        public static ProjectProperty ShouldEvaluatedEquivalentTo<T>(this ProjectProperty property, T value, string because = "", params object[] becauseArgs)
        {
            InternalShouldEvaluatedEquivalentTo(property.EvaluatedValue, value, property.Project, because, becauseArgs);

            return property;
        }

        private static void InternalShouldEvaluatedEquivalentTo<T>(string value, T expectedValue, Project project, string because = "", params object[] becauseArgs)
        {
            value
                .Should()
                .NotBeNull(because, becauseArgs);

            if (expectedValue is string selfEvaluatedValue)
            {
                expectedValue = (T)Convert.ChangeType(InternalReplaceWithProperty(selfEvaluatedValue, project.AllEvaluatedProperties.ToDictionary(p => p.Name, p => p.EvaluatedValue)), typeof(T), CultureInfo.InvariantCulture);
            }

            Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                .Should()
                .BeEquivalentTo(expectedValue, because, becauseArgs);
        }

        private static string InternalReplaceWithProperty(string template, IDictionary<string, string> properties)
        {
            var matches = Regex.Matches(template, @"\{([^\}]+)\}");
            foreach (Match property in matches)
            {
                template = template.Replace(property.Name, properties[property.Name], StringComparison.Ordinal);
            }

            return template;
        }
    }
}
