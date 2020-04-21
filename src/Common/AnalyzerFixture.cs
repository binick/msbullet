// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Flashy.Sdk.Tests.Utilities
{
    public abstract class AnalyzerFixture
        : BaseAnalyzerFixture
    {
        protected abstract DiagnosticAnalyzer CreateAnalyzer();

        protected async Task NoDiagnosticAsync(string code, string diagnosticId) => await this.NoDiagnosticAsync(this.Workspace.GetDocument(code, this.LanguageName), diagnosticId).ConfigureAwait(false);

        protected async Task NoDiagnosticAsync(Document document, string diagnosticId)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Assert.DoesNotContain(diagnosticId, (await this.GetDiagnosticsAsync(document).ConfigureAwait(false)).Select(p => p.Id));
        }

        protected async Task HasDiagnosticAsync(string markupCode, string diagnosticId)
        {
            Assert.True(this.TryGetDocumentAndSpanFromMarkup(markupCode, this.LanguageName, out Document document, out TextSpan span));

            await this.HasDiagnosticAsync(document, span, diagnosticId).ConfigureAwait(false);
        }

        protected async Task HasDiagnosticAsync(Document document, TextSpan span, string diagnosticId)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            ImmutableArray<Diagnostic> diagnostics = await this.GetDiagnosticsAsync(document).ConfigureAwait(false);
            Assert.Single(diagnostics);

            Diagnostic diagnostic = diagnostics.First();
            Assert.Equal(diagnosticId, diagnostic.Id);
            Assert.True(diagnostic.Location.IsInSource);
            Assert.Equal(span, diagnostic.Location.SourceSpan);
        }

        private async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(Document document)
        {
            var analyzers = ImmutableArray.Create(this.CreateAnalyzer());
            Compilation compilation = await document.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, cancellationToken: CancellationToken.None);
            compilation.GetDiagnostics(CancellationToken.None);

            foreach (Diagnostic diagnostic in await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false))
            {
                Location location = diagnostic.Location;
                if (location.IsInSource && location.SourceTree == (SyntaxTree)document.GetSyntaxTreeAsync(CancellationToken.None).Result)
                {
                    ImmutableArray.CreateBuilder<Diagnostic>().Add(diagnostic);
                }
            }

            return ImmutableArray.CreateBuilder<Diagnostic>().ToImmutable();
        }
    }
}
