using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AetheriumMono.Core;
using AetheriumMono.Data;
using AetheriumMono.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Content;
using tainicom.Aether.Physics2D.Dynamics;
namespace AetheriumMono.Scenes
{
    public class DefaultScene : IScene
    {
        BasicEffect spriteBatchEffect;
        BasicEffect polygonEffect;
        GraphicsDevice graphics;
        World physicsWorld;

        KeyboardState keyboard;
        KeyboardState previousKeyboard;

        const float PTU = 1f / 500;

        LiveContent liveContent;
        Dictionary<string, BodyTemplate> bodyTemplates;

        //List<GameObject> gameObjects = new List<GameObject>();
        Pool<GameObject> gameObjects = new Pool<GameObject>();

        //List<PhysicsObject> physicsObjects = new List<PhysicsObject>();
        List<CastRef<PhysicsObject>> physicsObjects = new List<CastRef<PhysicsObject>>(256);

        Vector3 cameraPosition;
        float cameraViewWidth = 20;
        CastRef<Ship> shipRef;
        bool renderColliders = true;

        public DefaultScene(GraphicsDevice graphics)
        {
            this.graphics = graphics;
        }

        public void Initialize()
        {
            liveContent = new LiveContent(graphics);
            Vector2 gravity = Vector2.Zero;
            physicsWorld = new World(gravity);
        }

        public void LoadContent(ContentManager content)
        {
            spriteBatchEffect = new BasicEffect(graphics);
            spriteBatchEffect.TextureEnabled = true;

            liveContent.ReadMetadata("Content.yaml");
            
            Texture2D shipTexture = liveContent.GetTexture("Hulls/ship.png");
            Texture2D squareTexture = liveContent.GetTexture("square.png");
            List<Vertices> shipPolygons = liveContent.GetPolygons("Hulls/ship.png");

            // Body templates
            bodyTemplates = PhysicsShapeLoader.LoadBodies(File.ReadAllText("./content/Bodies.xml"));

            // Scene setup
            var enemyShipRef = CreateShip(shipTexture, bodyTemplates["ship (2)"], new Vector2(-5, 0), shipTexture, bodyTemplates["square"], 100);

            shipRef = CreateShip(shipTexture, bodyTemplates["ship (2)"], new Vector2(0, 0), shipTexture, bodyTemplates["square"], 20);

            polygonEffect = new BasicEffect(graphics);
            polygonEffect.VertexColorEnabled = true;

            // TODO: fix this!
            var starTemplate = new GameObjectTemplate
            {
                Texture = squareTexture,
                Scale = new Vector2(.2f, .2f),
                Depth = 1
            };

            for (int i = 0; i < 10000; i++)
            {
                var squareRadius = 100;
                var (goRef, go) = starTemplate.Create(this);
                go.Position = new Vector2(Mathf.Random(-squareRadius, squareRadius), Mathf.Random(-squareRadius, squareRadius));
            }

        }

        public void Render(SpriteBatch spriteBatch)
        {
            // Calculate VP matrices
            var vp = graphics.Viewport;
            var view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
            var projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, -1, 1);
            spriteBatchEffect.View = view;
            spriteBatchEffect.Projection = projection;
            polygonEffect.View = view;
            polygonEffect.Projection = projection;

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, RasterizerState.CullClockwise, spriteBatchEffect);
            foreach (var go in gameObjects)
            {
                if (go.Texture == null) continue;
                var origin = new Vector2(go.Texture.Width * 0.5f, go.Texture.Height * 0.5f);
                spriteBatch.Draw(go.Texture, go.Position, null, Color.White, go.Rotation, origin, PTU * go.Scale, SpriteEffects.FlipVertically, go.Depth);
            }
            spriteBatch.End();

            if (renderColliders)
            {
                foreach (var poref in physicsObjects)
                {
                    if (!poref.Get(out var go)) continue;
                    var physicsObject = (PhysicsObject) go;

                    polygonEffect.World = Matrix.CreateRotationZ(physicsObject.Rotation) *
                                          Matrix.CreateTranslation(new Vector3(physicsObject.Position, 0));
                    polygonEffect.CurrentTechnique.Passes[0].Apply();
                    
                    VertexPositionColor[] vertices = physicsObject.Vertices;
                    short[] indices = physicsObject.Indices;
                    //graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, 
                    //    vertices, 0, vertices.Length, 
                    //    indices, 0, indices.Length / 3);

                    indices = physicsObject.LineIndices;
                    graphics.DrawUserIndexedPrimitives(PrimitiveType.LineList,
                        vertices, 0, vertices.Length,
                        indices, 0, indices.Length / 2);
                }
            }
        }


        public void Update(float deltaTime)
        {
            {
                if (shipRef.Get(out var ship))
                {
                    cameraPosition.X = ship.Position.X;
                    cameraPosition.Y = ship.Position.Y;
                }
            }

            previousKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            //Stopwatch sw = Stopwatch.StartNew();
            physicsWorld.Step(deltaTime);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            foreach (var poref in physicsObjects)
            {
                if (!poref.Get(out var go)) continue;
                var physicsObject = (PhysicsObject) go;

                var body = physicsObject.Body;

                physicsObject.Position = body.Position;
                physicsObject.Rotation = body.Rotation;
            }

            // Scene specific
            //square.Body.ApplyForce(Vector2.UnitX * 0.8f);

            {
                if (shipRef.Get(out var ship))
                {
                    ControlShip(ship);
                }
            }

            if (KeyJustPressed(Keys.F3))
            {
                renderColliders = !renderColliders;
            }

            gameObjects.EndOfFrame();
        }


        void ControlShip(Ship ship)
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
                // Strafe mode
                if (keyboard.IsKeyDown(Keys.A))
                    strafe -= 1;
                if (keyboard.IsKeyDown(Keys.D))
                    strafe += 1;
                if (ship.Body.AngularVelocity > 0)
                {
                    rotation = -1;
                }
                else if (ship.Body.AngularVelocity < 0)
                {
                    rotation = 1;
                }
            }
            else
            {
                // Rotate mode
                if (keyboard.IsKeyDown(Keys.A))
                    rotation += 1;
                if (keyboard.IsKeyDown(Keys.D))
                    rotation -= 1;
                //var horizontalMovement = Vector2.Dot(ship.Body.LinearVelocity, ship.Right);
                //if (horizontalMovement > 0)
                //{
                //    strafe = -Mathf.Clamp(Mathf.Abs(horizontalMovement), 0, 1);
                //}
                //else if (horizontalMovement < 0)
                //{
                //    strafe = Mathf.Clamp(Mathf.Abs(horizontalMovement), 0, 1);
                //}
            }
            ship.Control(forward, strafe, rotation);

            if (KeyJustPressed(Keys.Space))
                ship.Shoot();
        }

        public bool KeyJustPressed(Keys key)
        {
            return !previousKeyboard.IsKeyDown(key) && keyboard.IsKeyDown(key);
        }

        #region Creation Methods

        BodyTemplate TemplateFromVertices(List<Vertices> vertices, float density)
        {
            BodyTemplate body = new BodyTemplate();

            foreach (var polygon in vertices)
            {
                var fixture = new FixtureTemplate();
                fixture.Shape = new PolygonShape(polygon, density);
                body.Fixtures.Add(fixture);
            }

            return body;
        }

        CastRef<Ship> CreateShip(Texture2D shipTexture, BodyTemplate bodyTemplate, Vector2 position, Texture2D bulletTexture, BodyTemplate bulletTemplate, float health)
        {
            Ship ship = new Ship();
            ship.SetAssets(this, bulletTexture, bulletTemplate);
            ship.HealthAmount = health;

            PhysicsObjectTemplate template = new PhysicsObjectTemplate
            {
                Position = position,
                BodyTemplate = bodyTemplate,
                Texture = shipTexture,
                PhysicsObject = ship
            };

            return template.Create(this).Item1.Convert<Ship>();

        }

        public CastRef<PhysicsObject> SetupPhysicsObject(PhysicsObject physicsObject, BodyTemplate bodyTemplate)
        {
            var body = bodyTemplate.Create(physicsWorld);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;

            physicsObject.Body = body;

            var entityRef = SetupGameObject(physicsObject);
            var poRef = new CastRef<PhysicsObject>(entityRef);

            physicsObjects.Add(poRef);

            return poRef;
        }

        public void Destroy(EntityRef<GameObject> objectRef)
        {
            gameObjects.Remove(objectRef);
        }

        public void Destroy<T>(CastRef<T> castRef) where T : GameObject
        {
            gameObjects.Remove(castRef.EntityRef);
        }

        public EntityRef<GameObject> SetupGameObject(GameObject gameObject)
        {
            var entityRef = gameObjects.Create(gameObject);
            gameObject.Self = entityRef;
            return entityRef;
        }

        #endregion
    }
}
