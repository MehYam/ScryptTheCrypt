using System;
using System.Collections.Generic;

namespace ScryptTheCrypt
{
    public sealed class GameField
    {
        private List<GameActor> players = new List<GameActor>();
        private List<GameActor> mobs = new List<GameActor>();

        public GameField()
        {
        }
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
