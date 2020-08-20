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

            var collision = target.Body.Tag != Source;

            if (collision && target.Body.Tag is IHealth health)
            {
                health.TakeDamage(Damage);
            }

            if (collision)
            {
                Disabled = true;
                Self.Remove();
            }
            return collision;
        }

        public IBullet Clone()
        {
            return new Laser
            {
                Scene = Scene,
                Source = Source,
                Damage = Damage,
                Disabled = Disabled
            };
        }
    }
}
