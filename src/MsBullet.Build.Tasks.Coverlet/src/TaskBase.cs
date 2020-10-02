// See the LICENSE.TXT file in the project root for full license information.

using System;
using Microsoft.Build.Utilities;

namespace MsBullet.Build.Tasks.Coverlet
{
    public abstract class TaskBase : Task
    {
        private static IServiceProvider container;

        protected static IServiceProvider Container
        {
            get => TaskBase.container;
            set
            {
                if (TaskBase.container is null)
                {
                    TaskBase.container = value;
                }
            }
        }
    }
}
