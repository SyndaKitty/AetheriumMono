using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using System.Xml.XPath;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Content;

namespace AetheriumMono.Data
{
    public static class PhysicsShapeLoader
    {
        public static Dictionary<string, BodyTemplate> LoadBodies(string xml)
        {
            Dictionary<string, BodyTemplate> results = new Dictionary<string, BodyTemplate>();
            
            XDocument bodyXml = XDocument.Parse(xml);

            float ptm = 1f / (float)Convert.ToDouble(bodyXml.XPathSelectElement("bodydef/metadata/ptm_ratio").Value);

            var xmlBodies = bodyXml.XPathSelectElements("bodydef/bodies/body");
            foreach (var xmlBody in xmlBodies)
            {
                var bodyName = xmlBody.Attribute("name").Value;

                BodyTemplate body = new BodyTemplate();
                results.Add(bodyName, body);

                var xmlFixtures = xmlBody.XPathSelectElements("fixtures/fixture");
                foreach (var xmlFixture in xmlFixtures)
                {
                    float density = (float)Convert.ToDouble(xmlFixture.XPathSelectElement("density").Value);

                    var xmlPolygons = xmlFixture.XPathSelectElements("polygons/polygon");
                    foreach (var xmlPolygon in xmlPolygons)
                    {
                        Vertices vertices = new Vertices();
                        List<float> values = xmlPolygon.Value.Split(',')
                            .Select(x => (float) Convert.ToDouble(x.Trim())).ToList();

                        for (int i = 0; i < values.Count; i += 2)
                        {
                            vertices.Add(new Vector2(values[i], values[i + 1]) * ptm);
                        }

                        PolygonShape polygon = new PolygonShape(vertices, density);

                        FixtureTemplate fixture = new FixtureTemplate();
                        fixture.Shape = polygon;
                        body.Fixtures.Add(fixture);
                        body.Mass = 1;
                    }
                }
            }

            return results;
        }
    }
}
