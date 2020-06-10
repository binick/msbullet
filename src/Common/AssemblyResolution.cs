// Copyright (c) React Consulting, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

#if NET472
using System;
using System.IO;
using System.Reflection;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MsBullet
{
    internal static class AssemblyResolution
    {
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable IDE1006 // Naming Styles
        internal static TaskLoggingHelper Log;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1401 // Fields should be private

        public static void Initialize() => AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            if (!name.Name.Equals("System.Collections.Immutable", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Collections.Immutable.dll");

            Assembly sci;
            try
            {
                sci = Assembly.LoadFile(fullPath);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Log?.LogWarning($"AssemblyResolve: exception while loading '{fullPath}': {e.Message}");
                return null;
            }

            if (name.Version <= sci.GetName().Version)
            {
                Log?.LogMessage(MessageImportance.Low, $"AssemblyResolve: loaded '{fullPath}' to {AppDomain.CurrentDomain.FriendlyName}");
                return sci;
            }

            return null;
        }
    }
}
#endif
