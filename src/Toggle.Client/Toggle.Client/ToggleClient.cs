using System;
using System.Collections.Generic;
using System.Threading;
using Toggle.Client.Scheduler;
using Toggle.Logging;

namespace Toggle.Client
{
    public class ToggleClient : IDisposable
    {
        private static readonly ILog Logger = LogProvider.GetLogger(typeof(ToggleClient));

        private readonly Configuration _configuration;
        private readonly IFeatureToggleStore _featureToggleStore;
        private readonly IScheduledTaskManager _taskScheduler;
        
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken CancellationToken { get; }

        public ToggleClient(Configuration configuration)
        {
            Logger.Info($"Toggle Client: Initialised with SdkKey");
            
            CancellationToken = cancellationTokenSource.Token;

            _configuration = configuration;
            _featureToggleStore = new InMemoryFeatureToggleStore();
            var featureRequestor = new FeatureRequestor(configuration);
          
            var fetchTogglesTask = new FetchAllFeatureTogglesTask(_featureToggleStore, featureRequestor, configuration.PollingInterval);
            _taskScheduler = new ElapsedTimerScheduledTaskManager();
            _taskScheduler.Configure(_configuration, new List<IToggleScheduledTask>(){fetchTogglesTask}, CancellationToken);
        }
        
        public ToggleClient(string sdkKey) : this(Configuration.Default(sdkKey))
        {    
            
        }

        public bool IsEnabled(string toggleKey, bool defaultState)
        {
            try
            {
                var feature = _featureToggleStore.GetByKey(toggleKey);

                if (feature == null)
                    return defaultState;

                return feature.On;
            }
            catch (Exception e)
            {
                Logger.Error($"Fatal error trying to find toggle by key: {toggleKey}. With message {e.Message}");
            }

            return defaultState;
        }

        public void Dispose()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }
            
            _taskScheduler.Dispose();
        }
    }
}