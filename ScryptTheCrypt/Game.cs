using System;
using System.Collections.Generic;
using System.Text;

using ScryptTheCrypt.Utils;

namespace ScryptTheCrypt
{
    public sealed class Game
    {
        public enum ActorAlignment { Player, Mob };

        public readonly List<GameActor> players = new List<GameActor>();
        public readonly List<GameActor> mobs = new List<GameActor>();

        public readonly RNG rng;

        public Game(int seed = 0)
        {
            rng = new RNG(seed);
        }
        public void DoTurn()
        {
            GameEvents.Instance.TurnStart_Fire(this);

            // loop the actors, having them do their actions
            foreach (var actor in players)
            {
                actor.DoActions(this);
            }
            foreach (var actor in mobs)
            {
                actor.DoActions(this);
            }

            GameEvents.Instance.TurnEnd_Fire(this);
        }
    }
}
