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
                // basic mob spec, name, health
                var parts = kaiGameUtil.FileUtil.SplitAndTrim(line, ',');
                if (parts.Length < 4)
                {
                    throw new FormatException($"expecting >= 4 parts in {line}");
                }
                var iPart = 0;
                var mob = new GameActor(parts[iPart++], int.Parse(parts[iPart++]));
                var weapon = new GameWeapon(parts[iPart++], int.Parse(parts[iPart++]));
                mob.Weapon = weapon;

                ParseExtra(mob, parts, iPart);

                mobs.Add(mob);
            }
            return new GameMobGenerator(mobs);
        }
        // i.e. "tiger, 25, claw, 5, a:chooserandom, a:atk, a:choosestrongest, a:swipe:10:5"
        private static void ParseExtra(GameActor a, string[] parts, int iPart)
        {
            while (iPart < parts.Length)
            {
                var subParts = parts[iPart++].Split(':');
                if (subParts.Length < 2)
                {
                    throw new FormatException($"expecting more args in part ${iPart - 1} in {parts[iPart - 1]}");
                }
                switch(subParts[0])
                {

                }
            }
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
