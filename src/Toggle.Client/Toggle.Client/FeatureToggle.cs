namespace Toggle.Client
{
    public class FeatureToggle
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool On { get; set; }

        public FeatureToggle(string key, string name, bool on)
        {
            Key = key;
            Name = name;
            On = on;
        }
    }
}