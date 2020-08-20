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

        EntityRef<GameObject> SetupGameObject(GameObject gameObject);
        
        CastRef<PhysicsObject> SetupPhysicsObject(PhysicsObject physicsObject, BodyTemplate bodyTemplate);

    }
}
