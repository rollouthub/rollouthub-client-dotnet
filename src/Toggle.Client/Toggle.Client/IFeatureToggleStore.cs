namespace Toggle.Client
{
    public interface IFeatureToggleStore
    {
        void Initialise(TogglesResult toggles);
        FeatureToggle GetByKey(string key);
    }
}