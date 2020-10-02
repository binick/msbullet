// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Helpers;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    internal class FakeSourceRootTranslator : ISourceRootTranslator
    {
        public string ResolveFilePath(string originalFileName)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<SourceRootMapping> ResolvePathRoot(string pathRoot)
        {
            throw new NotImplementedException();
        }
    }
}
