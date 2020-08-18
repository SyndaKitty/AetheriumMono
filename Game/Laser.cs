using System;
using AetheriumMono.Core;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace AetheriumMono.Game
{
    public class Laser : PhysicsObject, IBullet
    {
        public IScene Scene { get; set; }
        public GameObject Source { get; set; }
        public float Damage { get; set; }
        public bool Disabled { get; set; }

        public bool OnCollision(Fixture bullet, Fixture target, Contact contact)
        {
            if (Disabled) return false;

            Console.WriteLine("OnCollision " + bullet.Body.Tag + " " + target.Body.Tag);

            var collision = target.Body.Tag != Source;

            if (collision && target.Body.Tag is IHealth health)
            {
                health.TakeDamage(Damage);
            }

            if (collision)
            {
                Disabled = true;
                Scene.Destroy(this);
            }
            return collision;

        }
    }
}
