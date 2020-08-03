using System;
using System.Collections.Generic;
using AetheriumMono.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Scenes
{
    public class DefaultScene : IScene
    {
        List<GameObject> gameObjects = new List<GameObject>(256);
        Texture2D texture;

        GameObject camera = new GameObject();

        float T = 0;

        GraphicsDeviceManager graphics;

        public DefaultScene(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public void Initialize()
        {
            
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Hulls/apogee");
            gameObjects.Add(new Ship{Texture = texture});
        }

        public void Render(SpriteBatch spriteBatch)
        {
            float w = graphics.GraphicsDevice.Viewport.Bounds.Width;
            float h = graphics.GraphicsDevice.Viewport.Bounds.Height;

            float x = w * 0.5f - camera.Position.X;
            float y = h * 0.5f -camera.Position.Y;

            var cameraMatrix = Matrix.CreateTranslation(x, y, 0);
            spriteBatch.Begin(transformMatrix: cameraMatrix);

            Console.WriteLine(cameraMatrix.ToString());

            foreach (var go in gameObjects)
            {
                if (go.Texture == null) continue;
                var origin = new Vector2(go.Texture.Width * 0.5f, go.Texture.Height * 0.5f);
                var position = go.Position;
                position.Y *= -1;
                spriteBatch.Draw(go.Texture, position, null, Color.White, go.Rotation, origin, Vector2.One, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        public void Update(float deltaTime)
        {
            T += deltaTime;
        }
    }
}
