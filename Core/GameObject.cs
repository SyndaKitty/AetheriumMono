using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Core
{
    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Offset;
        public Vector2 Scale = new Vector2(1, 1);

        public float Rotation;
        public Texture2D Texture;
    }
}
