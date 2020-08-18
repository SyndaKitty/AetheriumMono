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
using tainicom.Aether.Physics2D.Dynamics.Contacts;

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

        float T = 0;
        const float PTU = 1f / 500;
        const float UTP = 1f / PTU;

        LiveContent liveContent;
        Dictionary<string, BodyTemplate> bodyTemplates;

        List<GameObject> gameObjects = new List<GameObject>(256);
        List<PhysicsObject> physicsObjects = new List<PhysicsObject>(256);
        List<GameObject> destroyed = new List<GameObject>();
        Vector3 cameraPosition;
        float cameraViewWidth = 20;

        VertexBuffer vertexBuffer;

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

        VertexPositionTexture[] vert;
        short[] ind;

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
            //ship = CreateShip(shipTexture, TemplateFromVertices(shipPolygons, 1.0f), Vector2.Zero, shipTexture, TemplateFromVertices(shipPolygons, 1.0f));
            
            var ship = CreateShip(shipTexture, bodyTemplates["shiptest2"], new Vector2(0, 0), shipTexture, bodyTemplates["shiptest2"], 20);
            ship.Body.LocalCenter = Vector2.Zero;
            shipRef = new WeakReference<Ship>(ship);
            
            //square = SetupPhysicsObject(new PhysicsObject(), squareTexture, bodyTemplates["Square"], new Vector2(-10, 2.5f));

            polygonEffect = new BasicEffect(graphics);
            polygonEffect.VertexColorEnabled = true;

            for (int i = 0; i < 10000; i++)
            {
                var go = SetupGameObject(new GameObject(), squareTexture);
                go.Scale = new Vector2(.2f, .2f);
                var squareRadius = 500;
                go.Position = new Vector2(Mathf.Random(-squareRadius, squareRadius), Mathf.Random(-squareRadius, squareRadius));
            }

        }

        PhysicsObject square;

        WeakReference<Ship> shipRef;

        bool renderColliders = true;

        public void Render(SpriteBatch spriteBatch)
        {
            // Calculate VP matrices
            var vp = graphics.Viewport;
            var view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
            var projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0, 1);
            spriteBatchEffect.View = view;
            spriteBatchEffect.Projection = projection;
            polygonEffect.View = view;
            polygonEffect.Projection = projection;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, spriteBatchEffect);
            foreach (var go in gameObjects)
            {
                if (go.Texture == null) continue;
                var origin = new Vector2(go.Texture.Width * 0.5f, go.Texture.Height * 0.5f);
                spriteBatch.Draw(go.Texture, go.Position, null, Color.White, go.Rotation, origin, PTU * go.Scale, SpriteEffects.FlipVertically, 0);
            }
            spriteBatch.End();

            if (renderColliders)
            {
                foreach (var physicsObject in physicsObjects)
                {
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
                if (shipRef.TryGetTarget(out var ship))
                {
                    cameraPosition.X = ship.Position.X;
                    cameraPosition.Y = ship.Position.Y;
                }
            }

            previousKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            T += deltaTime;

            //Stopwatch sw = Stopwatch.StartNew();
            physicsWorld.Step(deltaTime);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            foreach (var physicsObject in physicsObjects)
            {
                var body = physicsObject.Body;

                physicsObject.Position = body.Position;
                physicsObject.Rotation = body.Rotation;
            }

            // Scene specific
            //square.Body.ApplyForce(Vector2.UnitX * 0.8f);

            {
                if (shipRef.TryGetTarget(out var ship))
                {
                    ControlShip(ship);
                }
            }

            //Console.WriteLine(ship.Body.AngularVelocity + " " + fakeAngularVelocity + " " + ship.Body.AngularVelocity / fakeAngularVelocity);

            if (destroyed.Count > 0)
            {
                Stopwatch sw = Stopwatch.StartNew();
                foreach (var destroyedGo in destroyed)
                {
                    // TODO: Dynamic reference system, tied in with a pooling system
                    if (shipRef.TryGetTarget(out var ship) && ship == destroyedGo)
                    {
                        shipRef.SetTarget(null);
                    }

                    gameObjects.Remove(destroyedGo);
                    if (destroyedGo is PhysicsObject po)
                    {
                        physicsObjects.Remove(po);
                        po.Body.Enabled = false;
                        physicsWorld.RemoveAsync(po.Body);
                    }
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                destroyed.Clear();
            }
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
                // strafe mode
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

        Ship CreateShip(Texture2D shipTexture, BodyTemplate bodyTemplate, Vector2 position, Texture2D bulletTexture, BodyTemplate bulletTemplate, float health)
        {
            Ship ship = new Ship();
            SetupPhysicsObject(ship, shipTexture, bodyTemplate);
            ship.SetAssets(this, bulletTexture, bulletTemplate);
            ship.HealthAmount = health;
            return ship;
        }

        Body CreateDynamicBody(BodyTemplate bodyTemplate)
        {
            var body = bodyTemplate.Create(physicsWorld);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;
            return body;
        }


        public PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate)
            => SetupPhysicsObject(physicsObject, texture, bodyTemplate, Vector2.Zero);

        public PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate, Vector2 position)
            => SetupPhysicsObject(physicsObject, texture, bodyTemplate, position, Vector2.One);

        public PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate, Vector2 position, Vector2 scale)
        {
            // Scale vertices of body template
            BodyTemplate bodyTemplateCopy = bodyTemplate.Scale(scale);

            physicsObject.Scale = scale;

            var body = CreateDynamicBody(bodyTemplateCopy);
            body.Position = position;
            physicsObject.Body = body;
            body.Tag = physicsObject;

            CalculateVertices(physicsObject);

            physicsObjects.Add(physicsObject);
            SetupGameObject(physicsObject, texture);
            return physicsObject;
        }

        public void Destroy(GameObject go)
        {
            destroyed.Add(go);
        }

        void CalculateVertices(PhysicsObject physicsObject)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            List<short> indices = new List<short>();
            List<short> lineIndices = new List<short>();

            int currentIndex = 0;

            foreach (var fixture in physicsObject.Body.FixtureList)
            {
                switch (fixture.Shape.ShapeType)
                {
                    case ShapeType.Circle:
                        break;
                    case ShapeType.Edge:
                        break;
                    case ShapeType.Polygon:
                        PolygonShape polygon = (PolygonShape)fixture.Shape;
                        for (int i = 0; i < polygon.Vertices.Count; i++)
                        {
                            var vertex = polygon.Vertices[i];
                            vertices.Add(new VertexPositionColor(new Vector3(vertex.X, vertex.Y, 0), new Color(255, 255, 255, 120)));
                        }

                        lineIndices.Add((short)currentIndex);
                        lineIndices.Add((short)(currentIndex + 1));
                        for (short i = 0; i < polygon.Vertices.Count - 2; i++)
                        {
                            lineIndices.Add((short) currentIndex);
                            lineIndices.Add((short)(currentIndex + i + 2));
                            lineIndices.Add((short)(currentIndex + i + 1));
                            lineIndices.Add((short) (currentIndex + i + 2));
                        }

                        for (short i = 0; i < polygon.Vertices.Count - 2; i++)
                        {
                            indices.Add((short)(currentIndex));
                            indices.Add((short)(currentIndex + i + 1));
                            indices.Add((short)(currentIndex + i + 2));
                        }
                        currentIndex += polygon.Vertices.Count;

                        break;
                    case ShapeType.Chain:
                        break;
                }
            }
             
            physicsObject.Vertices = vertices.ToArray();
            physicsObject.Indices = indices.ToArray();
            physicsObject.LineIndices = lineIndices.ToArray();
        }

        public GameObject SetupGameObject(GameObject gameObject, Texture2D texture)
        {
            gameObject.Texture = texture;
            gameObjects.Add(gameObject);
            return gameObject;
        }

        #endregion
    }
}
