using System;
using System.Collections.Generic;
using kaiGameUtil;

namespace ScryptTheCrypt
{
    public class GameMobGenerator
    {
        public readonly List<GameActor> mobSpec;
        private GameMobGenerator(List<GameActor> mobs)
        {
            mobSpec = mobs;
        }
        public static GameMobGenerator FromString(string spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec), "null mob spec");
            }
            var lines = kaiGameUtil.FileUtil.SplitStringIntoLines(spec);
            var mobs = new List<GameActor>();
            foreach (var line in lines)
            {
                var parts = kaiGameUtil.FileUtil.SplitAndTrim(line, ',');
                if (parts.Length < 4)
                {
                    throw new FormatException($"expecting 4 parts in {line}");
                }
                var mob = new GameActor(parts[0], int.Parse(parts[1]));
                var weapon = new GameWeapon(parts[2], int.Parse(parts[3]));
                mob.Weapon = weapon;
                mobs.Add(mob);
            }
            return new GameMobGenerator(mobs);
        }
        public List<GameActor> GenerateMobs(uint count, RNG rng)
        {
            if (mobSpec.Count < 1)
            {
                throw new InvalidOperationException("no mobs have been loaded");
            }
            var retval = new List<GameActor>();
            for (uint i = 0; i < count; ++i)
            {
                retval.Add(mobSpec[rng.Next(0, mobSpec.Count - 1)].Clone());
            }
            return retval;
        }
    }
}
