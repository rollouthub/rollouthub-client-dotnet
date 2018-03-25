using System;
using System.Linq;
using System.Threading;

namespace Toggle.Client
{
    public class InMemoryFeatureToggleStore : IFeatureToggleStore
    {
        private readonly ReaderWriterLockSlim ReadWritwLock = new ReaderWriterLockSlim();
        private static readonly int ReadWriteLockTimeout = 1000;
        private ToggleCollection Toggles = new ToggleCollection();
        
        public void Initialise(TogglesResult toggles)
        {
            ReadWritwLock.TryEnterWriteLock(ReadWriteLockTimeout);
            try
            {
                Toggles.Toggles.Clear();
                Toggles = toggles.ToggleCollection;
            }
            finally
            {
                ReadWritwLock.ExitWriteLock();
            }
        }

        public FeatureToggle GetByKey(string key)
        {
            ReadWritwLock.TryEnterReadLock(ReadWriteLockTimeout);
            
            try
            {
                if (Toggles == null)
                    return null;

                if (Toggles.Toggles.Count < 1)
                    return null;
                
                var featureToggle = Toggles.Toggles.First(x => x.Key == key);

                return featureToggle;
            }
            finally
            {
                ReadWritwLock.ExitReadLock();
            }
        }
    }
}