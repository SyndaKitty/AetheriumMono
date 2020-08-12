using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace AetheriumMono.Core
{
    public class PhysicsObject : GameObject
    {
        public Body Body { get; set; }
        public override Vector2 Position => Body.Position;
        public override float Rotation => Body.Rotation;
        public Vector2 Forward => new Vector2(-Mathf.Sin(Body.Rotation), Mathf.Cos(Body.Rotation));
        public Vector2 Right => new Vector2(Mathf.Cos(Body.Rotation), Mathf.Sin(Body.Rotation));
    }
}
