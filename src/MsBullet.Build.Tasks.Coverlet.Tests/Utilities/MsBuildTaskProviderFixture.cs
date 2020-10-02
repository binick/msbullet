// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Extensions;
using Coverlet.Core.Helpers;
using Microsoft.Build.Utilities;
using Xunit;
using Xunit.Abstractions;
using ILogger = Coverlet.Core.Abstractions.ILogger;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    [Collection(MsBuildTaskProviderCollection.Name)]
    public class MsBuildTaskProviderFixture : IDisposable
    {
        public MsBuildTaskProviderFixture()
        {
        }

        public static T Setup<T>(ITestOutputHelper output, params KeyValuePair<Type, object>[] services)
            where T : TaskBase, new()
        {
            var task = new T();

            var continer = new FakeServiceProvider(new Dictionary<Type, object>
            {
                { typeof(IInstrumentationHelper), new FakeInstrumentationHelper() },
                { typeof(ISourceRootTranslator), new FakeSourceRootTranslator() },
                { typeof(IFileSystem), new FakeFileSystem() },
                { typeof(ILogger), new FakeLogger(output) },
                { typeof(IRetryHelper), new RetryHelper() }
            }.Concat(services).ToDictionary(pair => pair.Key, pair => pair.Value));

            typeof(T).BaseType.GetField("container", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(task, continer);

            return task;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
