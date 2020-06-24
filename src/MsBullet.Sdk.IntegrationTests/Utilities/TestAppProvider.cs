// See the LICENSE.TXT file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit.Abstractions;

namespace MsBullet.Sdk.IntegrationTests.Utilities
{
    public class TestAppProvider
    {
        private readonly ConcurrentQueue<IDisposable> disposables;

        private readonly string workDir;
        private readonly string logOutputDir;
        private readonly string[] sourceDirectories;
        private readonly ICollection<KeyValuePair<string, string[]>> preCreateCommands;
        private readonly ICollection<KeyValuePair<string, string[]>> preBuildCommands;
        private readonly ICollection<KeyValuePair<string, string[]>> postBuildCommands;

        public TestAppProvider(string workDir, string logOutputDir, string[] sourceDirectories, ConcurrentQueue<IDisposable> disposables)
        {
            this.workDir = workDir;
            this.logOutputDir = logOutputDir;
            this.sourceDirectories = sourceDirectories;

            this.preBuildCommands = new Collection<KeyValuePair<string, string[]>>();
            this.preCreateCommands = new Collection<KeyValuePair<string, string[]>>();
            this.postBuildCommands = new Collection<KeyValuePair<string, string[]>>();

            this.disposables = disposables;
        }

        public TestAppProvider WithPreBuild(string command, params string[] commandArgs)
        {
            this.preBuildCommands.Add(KeyValuePair.Create(command, commandArgs));

            return this;
        }

        public TestAppProvider WithPostBuild(string command, params string[] commandArgs)
        {
            this.postBuildCommands.Add(KeyValuePair.Create(command, commandArgs));

            return this;
        }

        public TestAppProvider WithPreCreate(string command, params string[] commandArgs)
        {
            this.preCreateCommands.Add(KeyValuePair.Create(command, commandArgs));

            return this;
        }

        public TestApp Create(ITestOutputHelper output)
        {
            var app = new TestApp(this.workDir, this.logOutputDir, this.sourceDirectories);
            this.disposables.Enqueue(app);

            app.WireUp += (s, e) => Execute(output, (TestApp)s, this.preCreateCommands);
            app.PreBuild += (s, e) => Execute(output, (TestApp)s, this.preBuildCommands);
            app.PostBuild += (s, e) => Execute(output, (TestApp)s, this.postBuildCommands);

            return app;
        }

        private static void Execute(ITestOutputHelper output, TestApp app, IEnumerable<KeyValuePair<string, string[]>> commands)
        {
            foreach (var command in commands)
            {
                Execute(output, app, command.Key, command.Value);
            }
        }

        private static int Execute(ITestOutputHelper output, TestApp app, string command, params string[] commandArgs)
        {
            return app.UsafeExecutionScript(output, command, commandArgs);
        }
    }
}
