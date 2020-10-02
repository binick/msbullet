// See the LICENSE.TXT file in the project root for full license information.

using System;
using Coverlet.Core.Abstractions;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    internal class FakeInstrumentationHelper : IInstrumentationHelper
    {
        public void BackupOriginalModule(string module, string identifier)
        {
            throw new NotImplementedException();
        }

        public void DeleteHitsFile(string path)
        {
            throw new NotImplementedException();
        }

        public bool EmbeddedPortablePdbHasLocalSource(string module, out string firstNotFoundDocument)
        {
            throw new NotImplementedException();
        }

        public string[] GetCoverableModules(string module, string[] directories, bool includeTestAssembly)
        {
            throw new NotImplementedException();
        }

        public bool HasPdb(string module, out bool embedded)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalMethod(string method)
        {
            throw new NotImplementedException();
        }

        public bool IsModuleExcluded(string module, string[] excludeFilters)
        {
            throw new NotImplementedException();
        }

        public bool IsModuleIncluded(string module, string[] includeFilters)
        {
            throw new NotImplementedException();
        }

        public bool IsTypeExcluded(string module, string type, string[] excludeFilters)
        {
            throw new NotImplementedException();
        }

        public bool IsTypeIncluded(string module, string type, string[] includeFilters)
        {
            throw new NotImplementedException();
        }

        public bool IsValidFilterExpression(string filter)
        {
            throw new NotImplementedException();
        }

        public bool PortablePdbHasLocalSource(string module, out string firstNotFoundDocument)
        {
            throw new NotImplementedException();
        }

        public void RestoreOriginalModule(string module, string identifier)
        {
            throw new NotImplementedException();
        }

        public void SetLogger(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
