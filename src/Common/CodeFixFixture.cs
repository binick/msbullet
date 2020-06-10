// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace MsBullet.Sdk.Tests.Utilities
{
    public abstract class CodeFixFixture : BaseAnalyzerFixture
    {
        protected abstract CodeFixProvider CreateProvider();

        protected async Task TestCodeFix(string markupCode, string expected, DiagnosticDescriptor descriptor)
        {
            Assert.True(this.TryGetDocumentAndSpanFromMarkup(markupCode, this.LanguageName, out Document document, out TextSpan span));

            await this.TestCodeFixAsync(document, span, expected, descriptor).ConfigureAwait(false);
        }

        protected async Task TestCodeFixAsync(Document document, TextSpan span, string expected, DiagnosticDescriptor descriptor)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            ImmutableArray<CodeAction> codeFixes = await this.GetCodeFixesAsync(document, span, descriptor).ConfigureAwait(false);
            Assert.Single(codeFixes);

            await this.CodeActionAsync(codeFixes.First(), document, expected).ConfigureAwait(false);
        }

        private async Task<ImmutableArray<CodeAction>> GetCodeFixesAsync(Document document, TextSpan span, DiagnosticDescriptor descriptor)
        {
            ImmutableArray<CodeAction>.Builder builder = ImmutableArray.CreateBuilder<CodeAction>();

            SyntaxTree tree = await document.GetSyntaxTreeAsync(CancellationToken.None).ConfigureAwait(false);
            var diagnostic = Diagnostic.Create(descriptor, Location.Create(tree, span));
            var context = new CodeFixContext(document, diagnostic, (a, _) => builder.Add(a), CancellationToken.None);

            CodeFixProvider provider = this.CreateProvider();
            provider.RegisterCodeFixesAsync(context).Wait();

            return builder.ToImmutable();
        }

        private async Task CodeActionAsync(CodeAction codeAction, Document document, string expectedCode)
        {
            ImmutableArray<CodeActionOperation> operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            Assert.Single(operations);

            CodeActionOperation operation = operations.First();
            Workspace workspace = document.Project.Solution.Workspace;
            operation.Apply(workspace, CancellationToken.None);

            Document newDocument = workspace.CurrentSolution.GetDocument(document.Id);

            SourceText sourceText = await newDocument.GetTextAsync(CancellationToken.None).ConfigureAwait(false);
            string text = sourceText.ToString();

            Assert.Equal(expectedCode, text);
        }
    }
}
