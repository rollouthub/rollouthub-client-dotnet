using System.Collections.Generic;

namespace Toggle.Client
{
    public class ToggleCollection
    {
        private readonly Dictionary<string, FeatureToggle> _toggles;
        public ICollection<FeatureToggle> Toggles { get; }

        public ToggleCollection(ICollection<FeatureToggle> features = null)
        {
            Toggles = features ?? new List<FeatureToggle>(0);
            _toggles = new Dictionary<string, FeatureToggle>(Toggles.Count);

            foreach (var toggle in Toggles)
            {
                _toggles.Add(toggle.Key, toggle);
            }
        }
    }
}