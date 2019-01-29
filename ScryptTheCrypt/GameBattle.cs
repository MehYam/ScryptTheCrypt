using System.Collections;
using System.Collections.Generic;

using kaiGameUtil;

namespace ScryptTheCrypt
{
    public sealed class GameBattle
    {
        public enum ActorAlignment { Player, Mob };
        public enum Progress { InProgress, PlayersWin, MobsWin, Draw };

        public readonly List<GameActor> players = new List<GameActor>();
        public readonly List<GameActor> mobs = new List<GameActor>();

        public readonly RNG rng;

        public GameBattle(int seed = 0)
        {
            rng = new RNG(seed);
        }
        public GameBattle(RNG rng)
        {
            this.rng = rng;
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
        public IEnumerator EnumerateTurnActions()
        {
            GameEvents.Instance.TurnStart_Fire(this);
            foreach(var actor in players)
            {
                if (actor.Alive)
                {
                    var actions = actor.EnumerateActions(this);
                    while (actions.MoveNext())
                    {
                        yield return null;
                    }
                }
            }
            foreach(var actor in mobs)
            {
                if (actor.Alive)
                {
                    var actions = actor.EnumerateActions(this);
                    while (actions.MoveNext())
                    {
                        yield return null;
                    }
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
