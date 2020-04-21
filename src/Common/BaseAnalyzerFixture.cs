// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Flashy.Sdk.Tests.Utilities
{
    public abstract class BaseAnalyzerFixture
    {
        public BaseAnalyzerFixture() => this.Workspace = new DiagnosticWorkspace();

        protected abstract string LanguageName { get; }

        protected DiagnosticWorkspace Workspace { get; }

        protected bool TryGetCodeAndSpanFromMarkup(string markupCode, out string code, out TextSpan span)
        {
            if (string.IsNullOrEmpty(markupCode))
            {
                throw new ArgumentNullException(nameof(markupCode));
            }

            code = null;
            span = default;

            var builder = new StringBuilder();

            int start = markupCode.IndexOf("[|", StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return false;
            }

            builder.Append(markupCode.Substring(0, start));

            int end = markupCode.IndexOf("|]", StringComparison.OrdinalIgnoreCase);
            if (end < 0)
            {
                return false;
            }

            builder.Append(markupCode.Substring(start + 2, end - start - 2));
            builder.Append(markupCode.Substring(end + 2));

            code = builder.ToString();
            span = TextSpan.FromBounds(start, end - 2);

            return true;
        }

        protected bool TryGetDocumentAndSpanFromMarkup(string markupCode, string languageName, out Document document, out TextSpan span) => this.TryGetDocumentAndSpanFromMarkup(markupCode, languageName, null, out document, out span);

        protected bool TryGetDocumentAndSpanFromMarkup(string markupCode, string languageName, ImmutableList<MetadataReference> references, out Document document, out TextSpan span)
        {
            if (!this.TryGetCodeAndSpanFromMarkup(markupCode, out string code, out span))
            {
                document = null;
                return false;
            }

            document = this.Workspace.GetDocument(code, languageName, references);
            return true;
        }

    }
}
