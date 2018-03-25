using System;
using System.Collections.Generic;
using System.Threading;

namespace Toggle.Client.Scheduler
{
    public interface IScheduledTaskManager : IDisposable
    {
        void Configure(Configuration configuration, IEnumerable<IToggleScheduledTask> tasks, CancellationToken cancellationToken);
    }
}