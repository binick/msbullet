// See the LICENSE.TXT file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;

namespace MsBullet.Sdk.IntegrationTests
{
    public class InMemoryApp : TestApp
    {
        public InMemoryApp(string workDir, string logOutputDir, string[] sourceDirectories)
            : base(workDir, logOutputDir, sourceDirectories)
        {
        }

        public AdhocWorkspace Workspace { get; }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="code">Todo1.</param>
        /// <param name="languageName">Todo2.</param>
        /// <param name="references">Todo3.</param>
        /// <returns>Todo4.</returns>
#pragma warning disable CA1822 // Mark members as static
        public Document GetDocument(string code, string languageName = LanguageNames.CSharp, ImmutableList<MetadataReference> references = null)
#pragma warning restore CA1822 // Mark members as static
        {
            references ??= ImmutableList.Create<MetadataReference>(
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location));

#pragma warning disable CA2000 // Dispose objects before losing scope
            return new AdhocWorkspace()
#pragma warning restore CA2000 // Dispose objects before losing scope
                .AddProject("TestProject", languageName)
                .AddMetadataReferences(references)
                .AddDocument("TestDocument", code);
        }
    }
}
