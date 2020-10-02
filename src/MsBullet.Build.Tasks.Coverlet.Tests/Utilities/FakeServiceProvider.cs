// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MsBullet.Build.Tasks.Coverlet.Tests.Utilities
{
    public class FakeServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, object> services;

        public FakeServiceProvider(IDictionary<Type, object> services)
        {
            this.services = services;
        }

        public object GetService(Type serviceType)
        {
            return this.services[serviceType];
        }
    }
}
