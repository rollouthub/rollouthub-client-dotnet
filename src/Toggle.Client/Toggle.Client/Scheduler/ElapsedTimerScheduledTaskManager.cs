using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Toggle.Logging;

namespace Toggle.Client.Scheduler
{
    public class ElapsedTimerScheduledTaskManager : IScheduledTaskManager
    {
        private static readonly ILog Logger = LogProvider.GetLogger(typeof(ElapsedTimerScheduledTaskManager));
        private bool _disposed;
        private readonly Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        private static CancellationToken _cancellationToken;
        
        public void Configure(Configuration configuration, IEnumerable<IToggleScheduledTask> tasks, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            
            foreach (var scheduledTask in tasks)
            {
                var dueTime = scheduledTask.Interval;

                var callbackState = new CallbackState
                {
                    Name = scheduledTask.Name,
                    DueTime = dueTime,
                    Task = scheduledTask
                };

                var timer = new Timer(
                    callback: Callback,
                    state: callbackState,
                    dueTime: dueTime,
                    period: Timeout.InfiniteTimeSpan);

                timers.Add(scheduledTask.Name, timer);
                //Task.Run(() => TimerLoopAsync(scheduledTask, cancellationToken), cancellationToken);
            }
        }

        private async void Callback(object state)
        {
            if (!(state is CallbackState localState))
                return;
                
            if (!timers.TryGetValue(localState.Name, out var localTimer))
                return;

            var task = (state as CallbackState).Task;
            
            try
            {
                await task.ExecuteAsync(_cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Error($"Fatal error in timer callback for ${task.Name}");
            }
            finally
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    localTimer.SafeTimerChange(task.Interval, Timeout.InfiniteTimeSpan, ref _disposed);
                }
            }
        }

        private async Task TimerLoopAsync(IToggleScheduledTask scheduledTask, CancellationToken cancellationToken)
        {
            while (!_disposed)
            {
                await scheduledTask.ExecuteAsync(cancellationToken);
                await Task.Delay(scheduledTask.Interval, cancellationToken);
            }
        }
        
        

        public void Dispose()
        {
            Logger.Info("Disposing Toggle Scheduled Task Manager");
            if (_disposed)
                return;
            
            var timeout = TimeSpan.FromSeconds(1);
            
            using (var waitHandle = new ManualResetEvent(false))
            {
                foreach (var task in timers)
                {
                    if (task.Value.Dispose(waitHandle))
                    {
                        if (!waitHandle.WaitOne(timeout))
                        {
                            throw new TimeoutException($"CANARY_DEPLOY: Timeout waiting for task '{task.Key}' to stop..");
                        }
                    }
                }
            }
            _disposed = true;
            timers.Clear();
        }
        
        internal class CallbackState
        {
            public string Name { get; set; }
            public TimeSpan DueTime { get; set; }
            public IToggleScheduledTask Task { get; set; }
        }
    }
}