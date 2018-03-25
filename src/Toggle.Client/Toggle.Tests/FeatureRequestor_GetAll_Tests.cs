using System;
using System.Threading;
using Toggle.Client;
using Xunit;

namespace Toggle.Tests
{
    public class FeatureRequestor_GetAll_Tests
    {
        [Fact]
        public async void Success()
        {
            var requestor = BuildFeatureRequestor();

            var toggles = await requestor.GetAll(CancellationToken.None);
            
            Assert.True(toggles.Modified);
        }

        private FeatureRequestor BuildFeatureRequestor()
        {
            var configuration = Configuration.Default("dc8720db-8611-4747-a5e9-3a6aa85655b3");
            
            var requestor = new FeatureRequestor(configuration);

            return requestor;
        }
    }
}