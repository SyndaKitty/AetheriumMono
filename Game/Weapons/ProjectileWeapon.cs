namespace AetheriumMono.Game.Weapons
{
    public class ProjectileWeapon : IWeapon
    {
        // Seconds per bullet
        public float FireRate { get; set; }
        public IBullet Bullet { get; set; }
        
        float timeUntilShot;

        public void Shoot()
        {
            
        }

        public void Update()
        {
            
        }
    }
}
