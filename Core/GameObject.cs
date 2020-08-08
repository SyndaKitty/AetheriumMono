using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Core
{
    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Offset;
        public float Rotation;
        public Texture2D Texture;

        public GameObject(Texture2D texture)
        {
            Texture = texture;
            if (texture != null)
                Offset = new Vector2(texture.Width, texture.Height) * 0.5f;
        }
    }
}
