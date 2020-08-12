using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Core
{
    public class GameObject
    {
        public virtual Vector2 Position { get; set; }
        public Vector2 Offset;
        public Vector2 Scale = new Vector2(1, 1);

        public virtual float Rotation { get; set; }
        public Texture2D Texture;
    }
}
