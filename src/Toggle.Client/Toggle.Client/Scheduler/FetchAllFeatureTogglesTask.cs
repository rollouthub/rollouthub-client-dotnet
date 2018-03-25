using System;
using System.Threading;
using System.Threading.Tasks;
using Toggle.Logging;

namespace Toggle.Client.Scheduler
{
    public class FetchAllFeatureTogglesTask : IToggleScheduledTask
    {
        private static readonly ILog Logger = LogProvider.GetLogger(typeof(FetchAllFeatureTogglesTask));
        private readonly IFeatureToggleStore _featureToggleStore;
        private readonly IFeatureRequestor _featureRequestor;
        public string Name => "fetch-feature-toggles-task";

        public FetchAllFeatureTogglesTask(IFeatureToggleStore featureToggleStore, IFeatureRequestor featureRequestor, TimeSpan interval)
        {
            _featureToggleStore = featureToggleStore;
            _featureRequestor = featureRequestor;
            Interval = interval;
        }

        public TimeSpan Interval { get; }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _featureRequestor.GetAll(cancellationToken);

                if (!result.Modified)
                    return;

                _featureToggleStore.Initialise(result);
            }
            catch (UnsuccessfulResponseFromApiException exception)
            {
                Logger.Fatal($"Error from Toggle API {exception.StatusCode}");
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Error updating features: {exception.Message}");
            }
            
        }
    }
}