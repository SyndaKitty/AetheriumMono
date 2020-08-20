using AetheriumMono.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Data
{
    public class GameObjectTemplate
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale {get; set; }
        public Vector2? Offset { get; set; }
        public float Depth { get; set; }

        public (EntityRef<GameObject>, GameObject) Create(IScene scene)
        {
            var gameObject = new GameObject
            {
                Texture = Texture, 
                Position = Position, 
                Rotation = Rotation, 
                Scale = Scale,
                Depth = Depth
            };

            if (!Offset.HasValue)
            {
                Offset = new Vector2(Texture.Width, Texture.Height);
            }

            return (scene.SetupGameObject(gameObject), gameObject);
        }
    }
}