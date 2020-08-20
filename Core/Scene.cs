using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Content;

namespace AetheriumMono.Core
{
    public interface IScene
    {
        void Initialize();

        void LoadContent(ContentManager content);

        void Render(SpriteBatch spriteBatch);

        void Update(float deltaTime);

        EntityRef<GameObject> SetupGameObject(GameObject gameObject, Texture2D texture);
        
        CastRef<PhysicsObject> SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate);

        CastRef<PhysicsObject> SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate,
            Vector2 position);

        CastRef<PhysicsObject> SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate,
            Vector2 position, Vector2 scale);

    }
}
