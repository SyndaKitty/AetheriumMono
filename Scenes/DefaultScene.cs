using System;
using System.Collections.Generic;
using System.IO;
using AetheriumMono.Core;
using AetheriumMono.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Content;
using tainicom.Aether.Physics2D.Dynamics;

namespace AetheriumMono.Scenes
{
    public class DefaultScene : IScene
    {
        BasicEffect spriteBatchEffect;
        GraphicsDeviceManager graphics;
        World physicsWorld;

        KeyboardState keyboard;

        float T = 0;
        const float PTU = 1f / 500;
        const float UTP = 1f / PTU;

        // Objects
        List<GameObject> gameObjects = new List<GameObject>(256);
        List<PhysicsObject> physicsObjects = new List<PhysicsObject>(256);
        Vector3 cameraPosition;
        float cameraViewWidth = 20;

        // Content
        Texture2D apogeeTexture;
        Texture2D squareTexture;
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

        public void LoadContent(ContentManager content)
        {
            spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            spriteBatchEffect.TextureEnabled = true;

            // Textures
            apogeeTexture = content.Load<Texture2D>("Hulls/shiptest2");
            squareTexture = content.Load<Texture2D>("Hulls/Square");

            // Body templates
            bodyTemplates = PhysicsShapeLoader.LoadBodies(File.ReadAllText("./content/Bodies.xml"));

            // Scene setup
            ship = CreateShip(apogeeTexture, bodyTemplates["apogee"], Vector2.Zero);

            square = SetupPhysicsObject(new PhysicsObject(), squareTexture, bodyTemplates["Square"], new Vector2(-10, 2.5f));

            for (int i = 0; i < 100000; i++)
            {
                var go = SetupGameObject(new GameObject(), squareTexture);
                go.Scale = new Vector2(.2f, .2f);
                go.Position = new Vector2(Mathf.Random(-1000, 1000), Mathf.Random(-1000, 1000));
            }

        }

        PhysicsObject square;
        Ship ship;

        #region Creation Methods

        Ship CreateShip(Texture2D texture, BodyTemplate bodyTemplate, Vector2 position)
        {
            Ship ship = new Ship();
            SetupPhysicsObject(ship, texture, bodyTemplate);
            return ship;
        }

        Body CreateDynamicBody(BodyTemplate bodyTemplate)
        {
            var body = bodyTemplate.Create(physicsWorld);
            body.BodyType = BodyType.Dynamic;
            return body;
        }

        PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate)
            => SetupPhysicsObject(physicsObject, texture, bodyTemplate, Vector2.Zero);

        PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate, Vector2 position)
        {
            var body = CreateDynamicBody(bodyTemplate);
            body.Position = position;
            physicsObject.Body = body;

            physicsObjects.Add(physicsObject);
            SetupGameObject(physicsObject, texture);
            return physicsObject;
        }

        GameObject SetupGameObject(GameObject gameObject, Texture2D texture)
        {
            gameObject.Texture = texture;
            gameObjects.Add(gameObject);
            return gameObject;
        }

        #endregion

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
                spriteBatch.Draw(go.Texture, go.Position, null, Color.White, go.Rotation, origin, PTU * go.Scale, SpriteEffects.FlipVertically, 0);
            }
            spriteBatch.End();
        }

        public void Update(float deltaTime)
        {
            ship.Control(0, 0, 1);
            
            cameraPosition.X = ship.Position.X;
            cameraPosition.Y = ship.Position.Y;

            keyboard = Keyboard.GetState();

            T += deltaTime;
            physicsWorld.Step(deltaTime);

            foreach (var physicsObject in physicsObjects)
            {
                var body = physicsObject.Body;

                physicsObject.Position = body.Position;
                physicsObject.Rotation = body.Rotation;
            }

            // Scene specific
            square.Body.ApplyForce(Vector2.UnitX * 0.8f);
            //ControlShip();

            
            Console.WriteLine(ship.Body.AngularDamping);
            Console.WriteLine(T + " " + ship.Body.AngularVelocity * ship.Body.Inertia + " " + (ship.Body.AngularVelocity * ship.Body.Inertia - T));
        }

        void ControlShip()
        {
            float forward = 0;
            float strafe = 0;
            float rotation = 0;
            
            if (keyboard.IsKeyDown(Keys.W))
                forward += 1;
            if (keyboard.IsKeyDown(Keys.S))
                forward -= 1;
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                if (keyboard.IsKeyDown(Keys.A))
                    strafe -= 1;
                if (keyboard.IsKeyDown(Keys.D))
                    strafe += 1;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.A))
                    rotation += 1;
                if (keyboard.IsKeyDown(Keys.D))
                    rotation -= 1;    
            }
            ship.Control(forward, strafe, rotation);
        }
    }
}
