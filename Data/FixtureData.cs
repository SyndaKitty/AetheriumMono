namespace AetheriumMono.Data
{
    public struct FixtureData
    {
        public FixtureGroup FixtureGroup { get; set; }
    }

    public enum FixtureGroup
    {
        Default = 0,
        MainEngine = 1,
        LeftEngine = 2,
        RightEngine = 3
    }
}
