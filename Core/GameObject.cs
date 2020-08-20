using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Core
{
    public class GameObject
    {
        public virtual Vector2 Position { get; set; }
        public float Depth { get; set; }
        public Vector2 Offset;
        public virtual Vector2 Scale { get; set; } = new Vector2(1, 1);

        public virtual float Rotation { get; set; }
        public Texture2D Texture;
        public EntityRef<GameObject> Self { get; set; }
        
    }
}
