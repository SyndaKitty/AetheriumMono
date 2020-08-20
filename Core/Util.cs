using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Content;

namespace AetheriumMono.Core
{
    public static class Util
    {
        public static BodyTemplate Scale(this BodyTemplate bodyTemplate, Vector2 scale)
        {
            List<FixtureTemplate> fixtures = new List<FixtureTemplate>(bodyTemplate.Fixtures.Count);
            foreach (var fixture in bodyTemplate.Fixtures)
            {
                Shape shapeCopy;

                switch (fixture.Shape.ShapeType)
                {
                    case ShapeType.Polygon:
                        Vertices verticesCopy = new Vertices();
                        verticesCopy.AddRange(((PolygonShape)fixture.Shape).Vertices);
                        verticesCopy.Scale(scale);
                        shapeCopy = new PolygonShape(verticesCopy, fixture.Shape.Density);
                        break;
                    // TODO: support other shape types
                    default:
                        throw new NotSupportedException();
                }

                FixtureTemplate fixtureCopy = new FixtureTemplate();
                fixtureCopy.Shape = shapeCopy;
                fixtures.Add(fixtureCopy);
            }

            BodyTemplate newBodyTemplate = new BodyTemplate();
            newBodyTemplate.Fixtures = fixtures;

            return newBodyTemplate;
        }

        public static void RegisterRemovedCallback<T>(this EntityRef<T> entityRef, EntityRemoved<T> funcs) where T : class
        {
            entityRef.ParentPool.RegisterRemovedEvent(entityRef, funcs);
        }
    }
}
