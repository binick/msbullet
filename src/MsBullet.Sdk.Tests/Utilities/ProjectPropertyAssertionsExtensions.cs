// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace Microsoft.Build.Evaluation
{
    internal static class ProjectPropertyAssertionsExtensions
    {
        public static ProjectProperty ShouldEvaluatedEquivalentTo<T>(this ProjectProperty property, T value, string because = "", params object[] becauseArgs)
        {
            InternalShouldEquivalentTo(property.EvaluatedValue, value, property.Project, t => t, because, becauseArgs);

            return property;
        }

        public static ProjectProperty ShouldEvaluatedEquivalentTo<T>(this ProjectProperty property, T value, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config, string because = "", params object[] becauseArgs)
        {
            InternalShouldEquivalentTo(property.EvaluatedValue, value, property.Project, config, because, becauseArgs);

            return property;
        }

        public static ProjectProperty ShouldEvaluatedMatchRegex(this ProjectProperty property, string reqularExpression, string because = "", params object[] becauseArgs)
        {
            property.EvaluatedValue
                .Should()
                .MatchRegex(reqularExpression, because, becauseArgs);

            return property;
        }

        public static ProjectProperty ShouldUnevaluatedEquivalentTo<T>(this ProjectProperty property, T value, string because = "", params object[] becauseArgs)
        {
            InternalShouldEquivalentTo(property.UnevaluatedValue, value, property.Project, t => t, because, becauseArgs);

            return property;
        }

        public static ProjectProperty ShouldUnevaluatedEquivalentTo<T>(this ProjectProperty property, T value, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config, string because = "", params object[] becauseArgs)
        {
            InternalShouldEquivalentTo(property.UnevaluatedValue, value, property.Project, config, because, becauseArgs);

            return property;
        }

        public static ProjectProperty ShouldUnevaluatedEquivalentTo(this ProjectProperty property, string reqularExpression, string because = "", params object[] becauseArgs)
        {
            property.UnevaluatedValue
                .Should()
                .MatchRegex(reqularExpression, because, becauseArgs);

            return property;
        }

        private static void InternalShouldEquivalentTo<T>(string value, T expectedValue, Project project, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config, string because = "", params object[] becauseArgs)
        {
            value
                .Should()
                .NotBeNull(because, becauseArgs);

            if (expectedValue is string selfEvaluatedValue)
            {
                expectedValue = (T)Convert.ChangeType(InternalReplaceWithProperty(selfEvaluatedValue, project.AllEvaluatedProperties.ToLookup(p => p.Name, p => p.EvaluatedValue)), typeof(T), CultureInfo.InvariantCulture);
            }

            Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                .Should()
                .BeEquivalentTo(expectedValue, config, because, becauseArgs);
        }

        private static string InternalReplaceWithProperty(string template, ILookup<string, string> properties)
        {
            var matches = Regex.Matches(template, @"\{([^\}]+)\}");
            foreach (Match property in matches)
            {
                foreach (var value in properties[property.Name])
                {
                    template = template.Replace(property.Name, value, StringComparison.Ordinal);
                }
            }

            return template;
        }
    }
}
