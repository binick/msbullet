// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Evaluation.Context;
using Microsoft.CodeAnalysis;

using Project = Microsoft.Build.Evaluation.Project;

namespace MsBullet.Sdk.Tests.Utilities
{
    public class DiagnosticWorkspace
    {
        public DiagnosticWorkspace()
        {

        }

        public Document GetDocument(string code, string languageName = LanguageNames.CSharp, ImmutableList<MetadataReference> references = null)
        {
            references ??= ImmutableList.Create<MetadataReference>(
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.GetLocation()),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.GetLocation()));

            return new AdhocWorkspace()
                .AddProject("TestProject", languageName)
                .AddMetadataReferences(references)
                .AddDocument("TestDocument", code);
        }
    }
}
