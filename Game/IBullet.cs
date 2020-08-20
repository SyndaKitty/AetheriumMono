using AetheriumMono.Core;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace AetheriumMono.Game
{
    public interface IBullet
    {
        IScene Scene { get; set; }
        GameObject Source { get; set; }
        bool OnCollision(Fixture bullet, Fixture target, Contact contact);
        IBullet Clone();
    }
}
