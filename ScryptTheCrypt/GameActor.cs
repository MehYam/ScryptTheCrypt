using System;

namespace ScryptTheCrypt
{
    public sealed class GameActor
    {
        public float baseHealth { get; }
        public float health { get; private set; }
        public bool alive
        {
            get
            {
                return health > 0;
            }
        }
        public GameActor(float baseHealth)
        {
            if (baseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException("baseHealth", "baseHealth must be greater than zero");
            }
            this.baseHealth = baseHealth;
        }
        public void takeDamage(float d)
        {
            health = Math.Max(0, health - d);
        }
        public void heal(float h)
        {
            health = Math.Min(baseHealth, health + h);
        }
    }
}
