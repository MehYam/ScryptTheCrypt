﻿using System;
using System.Collections;
using System.Collections.Generic;

using KaiGameUtil;

namespace ScryptTheCrypt
{
    public sealed class Game
    {
        public enum Progress { InProgress, PlayersWin, MobsWin, Draw };

        public int NumRounds { get; private set; }
        public readonly RNG rng;

        public Game(int seed = 0) : this(new RNG(seed)) { }
        public Game(RNG rng)
        {
            this.rng = rng;
            NumRounds = 0;
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
        private readonly List<GameActor> players = new List<GameActor>();
        private readonly List<GameActor> mobs = new List<GameActor>();

        public IList<GameActor> Players { get { return players.AsReadOnly(); } }
        public IList<GameActor> Mobs { get { return mobs.AsReadOnly(); } }
        public void AddActor(GameActor actor)
        {
            var actors = actor.align == GameActor.Alignment.Player ? players : mobs;
            actors.Add(actor);
            GameEvents.Instance.ActorAdded_Fire(this, actor);
        }
        public void ClearActors(GameActor.Alignment align)
        {
            var actors = align == GameActor.Alignment.Player ? players : mobs;
            var actorsCopy = actors.ToArray();

            actors.Clear();
            foreach (var actor in actorsCopy)
            {
                GameEvents.Instance.ActorRemoved_Fire(this, actor);
            }
        }
        public IList<GameActor> GetTargets()
        {
            var targets = new List<GameActor>();
            targets.AddRange(players.FindAll(actor => actor.Targeted));
            targets.AddRange(mobs.FindAll(actor => actor.Targeted));
            return targets;
        }
        public Layer<GameActor> GetPositions()
        {
            var min = PointUtil.max;
            var max = PointUtil.min;
            void RecordBounds(IList<GameActor> actors)
            {
                foreach (var actor in players)
                {
                    if (actor.pos.x < min.x)
                    {
                        min.x = actor.pos.x;
                    }
                    if (actor.pos.y < min.y)
                    {
                        min.y = actor.pos.y;
                    }
                    if (actor.pos.x > max.x)
                    {
                        max.x = actor.pos.x;
                    }
                    if (actor.pos.x > max.y)
                    {
                        max.y = actor.pos.y;
                    }
                }
            }
            RecordBounds(players);
            RecordBounds(mobs);
            if (max != PointUtil.min)
            {
                var positions = new Layer<GameActor>(min, max);
                Action<GameActor> setter = actor => { positions.Set(actor.pos, actor); };
                players.ForEach(setter);
                mobs.ForEach(setter);
                return positions;
            }
            return null;
        }
        public void PlayRound()
        {
            ++NumRounds;
            GameEvents.Instance.RoundStart_Fire(this);

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

            GameEvents.Instance.RoundEnd_Fire(this);
        }
        public IEnumerator EnumerateRound()
        {
            ++NumRounds;
            GameEvents.Instance.RoundStart_Fire(this);
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
            GameEvents.Instance.RoundEnd_Fire(this);
        }
        public void PlayRound_Scrypt()
        {
            ++NumRounds;
            GameEvents.Instance.RoundStart_Fire(this);

            // loop the actors, having them do their actions
            foreach (var actor in players)
            {
                if (actor.Alive)
                {
                    actor.RunScrypt(this);
                }
            }
            foreach (var actor in mobs)
            {
                if (actor.Alive)
                {
                    actor.RunScrypt(this);
                }
            }

            GameEvents.Instance.RoundEnd_Fire(this);
        }
        public IEnumerator EnumerateRound_Scrypt()
        {
            ++NumRounds;
            GameEvents.Instance.RoundStart_Fire(this);
            foreach(var actor in players)
            {
                if (actor.Alive)
                {
                    actor.RunScrypt(this);
                    yield return null;
                }
            }
            foreach(var actor in mobs)
            {
                if (actor.Alive)
                {
                    actor.RunScrypt(this);
                    yield return null;
                }
            }
            GameEvents.Instance.RoundEnd_Fire(this);
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
