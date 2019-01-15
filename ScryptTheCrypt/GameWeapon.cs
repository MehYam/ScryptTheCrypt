using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt
{
    public sealed class GameWeapon
    {
        public readonly string name;
        public readonly float damage;
        public GameWeapon(string name, float damage)
        {
            if (name == null) throw new ArgumentNullException("name", "name cannot be null");
            if (damage < 0) throw new ArgumentOutOfRangeException("damage", "damage must be positive");

            this.name = name;
            this.damage = damage;
        }
    }
}
