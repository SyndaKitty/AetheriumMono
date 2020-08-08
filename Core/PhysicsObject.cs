using tainicom.Aether.Physics2D.Dynamics;

namespace AetheriumMono.Core
{
    public class PhysicsObject
    {
        public GameObject GameObject { get; private set; }
        public Body Body { get; set; }

        public PhysicsObject(GameObject gameObject, Body body)
        {
            GameObject = gameObject;
            Body = body;
        }
    }
}
