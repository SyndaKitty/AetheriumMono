using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.Decomposition;
using tainicom.Aether.Physics2D.Common.PolygonManipulation;
using YamlDotNet.Serialization;
using Path = System.IO.Path;

namespace AetheriumMono.Core
{
    public class LiveContent : IDisposable
    {
        Dictionary<string, Asset> assetMetadata = new Dictionary<string, Asset>();

        Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        Dictionary<string, List<Vertices>> polygonCache = new Dictionary<string, List<Vertices>>();

        GraphicsDevice graphicsDevice;

        const string ContentPath = "./Content/";

        public LiveContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void ReadMetadata(string file)
        {
            string path = Path.Combine(ContentPath, file);
            assetMetadata.Clear();
            
            string yaml = File.ReadAllText(path);

            var deserializer = new DeserializerBuilder().Build();
            var metadata = (Dictionary<Object, Object>) deserializer.Deserialize(new StringReader(yaml));

            foreach (var assetKey in metadata.Keys)
            {
                var asset = new Asset
                {
                    Key = assetKey.ToString().ToLower(),
                    ImportMethods = new List<ImportMethod>()
                };

                var methods = (Dictionary<Object, Object>)metadata[assetKey];

                foreach (var method in methods.Keys)
                {
                    asset.ImportMethods.Add(new ImportMethod
                    {
                        Name = method.ToString(),
                        ImportData = new ImportData {Data = methods[method] as Dictionary<object, object>}
                    });
                }
                assetMetadata.Add(assetKey.ToString().ToLower(), asset);
            }
        }

        public void LoadAsset(string assetKey)
        {
            assetKey = assetKey.ToLower();
            if (!assetMetadata.ContainsKey(assetKey))
            {
                throw new InvalidDataException($"Asset info for key {assetKey} not provided");
            }

            var asset = assetMetadata[assetKey];
            foreach (var method in asset.ImportMethods)
            {
                switch (method.Name)
                {
                    case "texture": 
                        LoadTexture(assetKey, method.ImportData);
                        break;
                    //case "texturePolygons":
                    //    var texture = textureCache[assetKey];
                    //    LoadPolygons(assetKey, texture, method.ImportData);
                    //    break;

                    default:
                        throw new NotImplementedException(method.Name + " import method not implemented");
                }
            }
        }

        #region Textures
        void LoadTexture(string assetKey, ImportData importData)
        {
            assetKey = assetKey.ToLower();
            string path = Path.Combine(ContentPath, assetKey);
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Texture2D texture = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();

            textureCache[assetKey] = texture;
        }

        public Texture2D GetTexture(string assetKey)
        {
            assetKey = assetKey.ToLower();
            if (textureCache.TryGetValue(assetKey, out var cachedTexture))
            {
                return cachedTexture;
            }

            LoadAsset(assetKey);
            return textureCache[assetKey];
        }
        #endregion Textures

        #region Polygons
        //void LoadPolygons(string assetKey, Texture2D texture, ImportData importData)
        //{
        //    assetKey = assetKey.ToLower();
        //    float scaleFactor = 1f / importData.GetInt("scaleFactor");
        //    bool detectHoles = importData.GetBool("detectHoles");
        //    float tolerance = importData.GetFloat("tolerance");

        //    //Create an array to hold the data from the texture
        //    uint[] textureData = new uint[texture.Width * texture.Height];

        //    //Transfer the texture data to the array
        //    texture.GetData(textureData);

        //    //Find the vertices that makes up the outline of the shape in the texture
        //    Vertices outline = PolygonTools.CreatePolygon(textureData, texture.Width, detectHoles);
        //    Vector2 centroid = -outline.GetCentroid();
        //    outline.Translate(ref centroid);
        //    outline = SimplifyTools.DouglasPeuckerSimplify(outline, tolerance);
        //    List<Vertices> result = Triangulate.ConvexPartition(outline, TriangulationAlgorithm.Bayazit);
        //    Vector2 scale = new Vector2(scaleFactor, -scaleFactor);
        //    foreach (Vertices vertices in result)
        //    {
        //        vertices.Scale(ref scale);
        //    }

        //    polygonCache[assetKey] = result;
        //}

        //public List<Vertices> GetPolygons(string assetKey)
        //{
        //    assetKey = assetKey.ToLower();
        //    if (polygonCache.TryGetValue(assetKey, out var cachedPolygons))
        //    {
        //        return cachedPolygons;
        //    }

        //    LoadAsset(assetKey);
        //    return polygonCache[assetKey];
        //}
        #endregion Polygons

        public void Dispose()
        {
            textureCache.Clear();
            polygonCache.Clear();
        }
    }

    public class Asset
    {
        public string Key { get; set;}
        public List<ImportMethod> ImportMethods { get; set; }
    }

    public class ImportMethod
    {
        public string Name { get; set; }
        public ImportData ImportData { get; set; }
    }

    public class ImportData
    {
        public Dictionary<Object, Object> Data { get; set; }

        public bool GetBool(string key)
        {
            return Convert.ToBoolean(Data[key]);
        }

        public int GetInt(string key)
        {
            return Convert.ToInt32(Data[key]);
        }

        public float GetFloat(string key)
        {
            return (float)Convert.ToDouble(Data[key]);
        }
    }
}
