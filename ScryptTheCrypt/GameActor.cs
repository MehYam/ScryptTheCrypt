using System;

namespace ScryptTheCrypt
{
    public sealed class GameActor
    {
        public readonly float baseHealth;
        public float health { get; private set; }
        public GameWeapon weapon { get; set; }
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
            this.health = baseHealth;
        }
        public void takeDamage(float d)
        {
            health = Math.Max(0, health - d);
        }
        public void heal(float h)
        {
            health = Math.Min(baseHealth, health + h);
        }
        public void dealDamage(GameActor other)
        {
            if (other == null) throw new ArgumentNullException("other", "other actor is null");
            if (weapon != null)
            {
                other.takeDamage(weapon.damage);
            }
        }
    }
}
