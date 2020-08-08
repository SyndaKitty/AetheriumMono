using System;
using System.Collections.Generic;
using System.IO;
using AetheriumMono.Core;
using AetheriumMono.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Content;
using tainicom.Aether.Physics2D.Dynamics;

namespace AetheriumMono.Scenes
{
    public class DefaultScene : IScene
    {
        BasicEffect spriteBatchEffect;
        GraphicsDeviceManager graphics;
        World physicsWorld;
        
        float T = 0;
        const float PTU = 1f / 100;
        const float UTP = 1f / PTU;

        // Objects
        List<GameObject> gameObjects = new List<GameObject>(256);
        List<PhysicsObject> physicsObjects = new List<PhysicsObject>(256);
        Vector3 cameraPosition;
        float cameraViewWidth = 20;

        // Content
        Texture2D apogeeTexture;
        Dictionary<string, BodyTemplate> bodyTemplates;

        public DefaultScene(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public void Initialize()
        {
            Vector2 gravity = Vector2.Zero;
            physicsWorld = new World(gravity);
        }

        PhysicsObject ship;

        public void LoadContent(ContentManager content)
        {
            spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            spriteBatchEffect.TextureEnabled = true;

            apogeeTexture = content.Load<Texture2D>("Hulls/apogee");
            var squareTexture = content.Load<Texture2D>("Hulls/Square");
            bodyTemplates = PhysicsShapeLoader.LoadBodies(File.ReadAllText("./content/Bodies.xml"));

            Body body = bodyTemplates["apogee"].Create(physicsWorld);
            body.BodyType = BodyType.Dynamic;
            
            ship = CreatePhysicsObject(apogeeTexture, body);
            ship.GameObject.Texture = apogeeTexture;
            //ship.Body.ApplyForce(Vector2.One * 1000);

            Body body2 = bodyTemplates["Square"].Create(physicsWorld);
            body2.BodyType = BodyType.Dynamic;
            square = CreatePhysicsObject(squareTexture, body2);
            square.GameObject.Texture = squareTexture;
            //ship.GameObject.Position.X = -1;
            square.Body.Position = new Vector2(-10, 0);
        }

        PhysicsObject square;

        GameObject CreateGameObject(Texture2D texture)
        {
            GameObject go = new GameObject(texture);
            gameObjects.Add(go);
            return go;
        }

        PhysicsObject CreatePhysicsObject(Texture2D texture, Body body)
        {
            var go = CreateGameObject(texture);
            var po = new PhysicsObject(go, body);
            physicsObjects.Add(po);
            go.Offset += body.LocalCenter / PTU;

            return po;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            var vp = graphics.GraphicsDevice.Viewport;
            spriteBatchEffect.View = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
            spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0, 1);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, spriteBatchEffect);
            foreach (var go in gameObjects)
            {
                if (go.Texture == null) continue;
                var origin = new Vector2(go.Texture.Width * 0.5f, go.Texture.Height * 0.5f);
                spriteBatch.Draw(go.Texture, go.Position, null, Color.White, go.Rotation, origin, Vector2.One * PTU, SpriteEffects.FlipVertically, 0);
            }
            spriteBatch.End();
        }

        public void Update(float deltaTime)
        {
            square.Body.ApplyForce(Vector2.UnitX * 10);

            T += deltaTime;
            physicsWorld.Step(deltaTime);

            foreach (var physicsObject in physicsObjects)
            {
                var go = physicsObject.GameObject;
                var body = physicsObject.Body;
                //Console.WriteLine(body.Position + " " + body.Rotation + " m:" + body.
                //);
                go.Position = body.Position;
                go.Rotation = body.Rotation;
            }
        }
    }
}
