using System;
using System.Collections.Generic;

namespace ScryptTheCrypt
{
    public sealed class GameField
    {
        private readonly List<GameActor> players = new List<GameActor>();
        private readonly List<GameActor> mobs = new List<GameActor>();

        public void addPlayer(GameActor a)
        {
            players.Add(a);
        }
        public void addMob(GameActor m)
        {
            mobs.Add(m);
        }
    }
}
