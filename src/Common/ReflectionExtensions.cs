// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace MsBullet.Sdk.Tests.Utilities
{
    internal static class ReflectionExtensions
    {
        public static string GetLocation(this Assembly assembly) => AssemblyLightUp._get_Location == null
            ? throw new PlatformNotSupportedException()
            : AssemblyLightUp._get_Location(assembly);

        public static T CreateDelegate<T>(this MethodInfo methodInfo) => methodInfo == null
            ? default
            : (T)(object)methodInfo.CreateDelegate(typeof(T));

        private static class AssemblyLightUp
        {
            internal static readonly Type Type = typeof(Assembly);

            internal static readonly Func<Assembly, string> _get_Location = Type
                .GetTypeInfo()
                .GetDeclaredMethod("get_Location")
                .CreateDelegate<Func<Assembly, string>>();
        }
    }
}
