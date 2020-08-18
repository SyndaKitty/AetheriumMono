using AetheriumMono.Core;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace AetheriumMono.Game
{
    public class Laser : PhysicsObject, IBullet
    {
        public IScene Scene { get; set; }
        public GameObject Source { get; set; }

        public bool OnCollision(Fixture bullet, Fixture target, Contact contact)
        {
            Scene.Destroy(this);
            return target.Body.Tag != Source;
        }
    }
}
