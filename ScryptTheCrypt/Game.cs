using System;
using System.Collections.Generic;
using System.Text;

using ScryptTheCrypt.Utils;

namespace ScryptTheCrypt
{
    public sealed class Game
    {
        //KAI: what's the difference between Game and GameField...
        public readonly List<GameActor> players = new List<GameActor>();
        public readonly List<GameActor> mobs = new List<GameActor>();

        public readonly RNG rng;

        public Game(int seed)
        {
            rng = new RNG(seed);
        }
        public void DoTurn()
        {
            // loop the actors, having them do their actions
            foreach (var actor in players)
            {
                actor.DoActions(this);
            }
            foreach (var actor in mobs)
            {
                actor.DoActions(this);
            }
        }
    }
}
