using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toggle.Client.Scheduler
{
    public interface IToggleScheduledTask
    {
        TimeSpan Interval { get; }
        string Name { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}