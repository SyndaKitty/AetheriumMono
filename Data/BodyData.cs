using System.Collections.Generic;
using tainicom.Aether.Physics2D.Content;

namespace AetheriumMono.Data
{
    public class BodyData
    {
        public BodyTemplate Template { get; set; }
        public List<FixtureData> FixtureData { get; set; } = new List<FixtureData>();
    }
}
