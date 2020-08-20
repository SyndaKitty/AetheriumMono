using System.Collections.Generic;
using AetheriumMono.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Content;

namespace AetheriumMono.Data
{
    public class PhysicsObjectTemplate
    {
        public Texture2D Texture { get; set; }
        public BodyTemplate BodyTemplate { get; set; }
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; }
        public float Depth { get; set; }
        public Vector2? Offset { get; set; }
        
        // For dynamic templates
        public PhysicsObject PhysicsObject { get; set; }

        public (CastRef<PhysicsObject>, PhysicsObject) Create(IScene scene)
        {
            // Scale vertices of body template
            BodyTemplate bodyTemplateCopy = BodyTemplate.Scale(Scale);

            PhysicsObject physicsObject = PhysicsObject ?? new PhysicsObject();

            physicsObject.Texture = Texture;
            physicsObject.Scale = Scale;
            
            CastRef<PhysicsObject> poref = scene.SetupPhysicsObject(physicsObject, bodyTemplateCopy);
            
            physicsObject.Body.Position = Position;
            physicsObject.Body.Rotation = Rotation;
            physicsObject.Depth = Depth;
            
            physicsObject.Body.Tag = physicsObject;
            physicsObject.Body.LocalCenter = Vector2.Zero;

            CalculateVertices(physicsObject);

            if (!Offset.HasValue)
            {
                physicsObject.Offset = new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f);
            }

            return (poref, physicsObject);
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
    }
}
