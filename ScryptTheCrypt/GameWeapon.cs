using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt
{
    public sealed class GameWeapon
    {
        public readonly string name;
        public readonly float damage;
        public readonly float critMultiplier;
        public GameWeapon(string name, float damage)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name), "name cannot be null");
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "damage must be positive");
            }
            this.damage = damage;
            this.critMultiplier = 2;
        }
        public override string ToString()
        {
            return $"{name}, damage {damage}, crit x{critMultiplier}";
        }
    }
}
