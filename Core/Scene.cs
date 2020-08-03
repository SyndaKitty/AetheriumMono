using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AetheriumMono.Core
{
    public interface IScene
    {
        void Initialize();

        void LoadContent(ContentManager content);

        void Render(SpriteBatch spriteBatch);

        void Update(float deltaTime);
    }
}
