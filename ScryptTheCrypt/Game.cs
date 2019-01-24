using System;
using System.Collections.Generic;
using System.Text;

using ScryptTheCrypt.Utils;

namespace ScryptTheCrypt
{
    public sealed class Game
    {
        public enum ActorAlignment { Player, Mob };
        public enum Progress { InProgress, PlayersWin, MobsWin, Draw };

        public readonly List<GameActor> players = new List<GameActor>();
        public readonly List<GameActor> mobs = new List<GameActor>();

        public readonly RNG rng;

        public Game(int seed = 0)
        {
            rng = new RNG(seed);
        }
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"seed {rng.seed}");
            sb.Append("--players--");
            foreach (var player in players)
            {
                sb.AppendLine();
                sb.Append(player.ToString());
            }
            sb.Append("\n--mobs--");
            foreach (var mobs in mobs)
            {
                sb.AppendLine();
                sb.Append(mobs.ToString());
            }
            return sb.ToString();
        }
        public void DoTurn()
        {
            GameEvents.Instance.TurnStart_Fire(this);

            // loop the actors, having them do their actions
            foreach (var actor in players)
            {
                if (actor.Alive)
                {
                    actor.DoActions(this);
                }
            }
            foreach (var actor in mobs)
            {
                if (actor.Alive)
                {
                    actor.DoActions(this);
                }
            }

            GameEvents.Instance.TurnEnd_Fire(this);
        }
        public Progress GameProgress {
            get
            {
                bool playersAlive = players.Exists(player => player.Alive);
                bool mobsAlive = mobs.Exists(mob => mob.Alive);

                if (!playersAlive && !mobsAlive)
                {
                    return Progress.Draw;
                }
                if (!playersAlive)
                {
                    return Progress.MobsWin;
                }
                if (!mobsAlive)
                {
                    return Progress.PlayersWin;
                }
                return Progress.InProgress;
            }
        }
    }
}
