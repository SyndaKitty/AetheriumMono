namespace AetheriumMono.Game
{
    public interface IHealth
    {
        float HealthAmount { get; set; }
        void TakeDamage(float amount);
    }
}
