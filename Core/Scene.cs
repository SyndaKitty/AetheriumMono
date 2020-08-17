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

        GameObject SetupGameObject(GameObject gameObject, Texture2D texture);
        
        PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate);

        PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate,
            Vector2 position);

        PhysicsObject SetupPhysicsObject(PhysicsObject physicsObject, Texture2D texture, BodyTemplate bodyTemplate,
            Vector2 position, Vector2 scale);

    }
}
