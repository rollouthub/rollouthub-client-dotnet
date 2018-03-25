using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toggle.Client;
using Toggle.Client.Scheduler;
using Xunit;

namespace Toggle.Tests
{
    public class ElapsedTimerScheduledTaskManager_Evaluate_Tests
    {
        class TestTask : IToggleScheduledTask
        {
            public TimeSpan Interval { get; }
            public string Name => "test-task";
            public int Counter { get; set; } = 1;
            public TimeSpan ExecutionDelay {get;set;}
            public ManualResetEventSlim Reset { get; set; }
            
            public async Task ExecuteAsync(CancellationToken cancellationToken)
            {
                Counter++;

                await Task.Delay(ExecutionDelay, cancellationToken);

                if (Counter == 5)
                    Reset?.Set();
            }
        }
        [Fact]
        public void Success()
        {
            using (var reset = new ManualResetEventSlim(false))
            using (var taskManager = new ElapsedTimerScheduledTaskManager())
            {
                var task = new TestTask()
                {
                    ExecutionDelay = TimeSpan.FromMilliseconds(10),
                    Reset = reset
                };
                var configuration = Configuration.Default("12345");
                taskManager.Configure(configuration, new List<IToggleScheduledTask>() { task }, CancellationToken.None);

                reset.Wait();
                Assert.Equal(task.Counter, 5);
            }
        }
    }
}