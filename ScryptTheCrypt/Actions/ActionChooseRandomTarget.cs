using System;
using System.Collections.Generic;

namespace ScryptTheCrypt.Actions
{
    public sealed class ActionChooseRandomTarget : IActorAction
    {
        readonly Game.ActorAlignment targetAlignment;
        public ActionChooseRandomTarget(Game.ActorAlignment targetAlignment)
        {
            this.targetAlignment = targetAlignment;
        }
        public void act(Game g, GameActor actor)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }
            GameActor selectLiving(IList<GameActor> actors)
            {
                //KAI: readonlycollection doesn't have FindAll... LINQ would clean this up
                var nondead = new List<GameActor>(actors).FindAll(a => a.Alive);
                return nondead.Count > 0 ? nondead[g.rng.NextIndex(nondead)] : null;
            }
            GameActor target = null;
            switch(targetAlignment)
            {
                case Game.ActorAlignment.Mob:
                    target = selectLiving(g.Mobs);
                    break;
                case Game.ActorAlignment.Player:
                    target = selectLiving(g.Players);
                    break;
            }

            actor.target = target;
            if (actor.target != null)
            {
                GameEvents.Instance.TargetChosen_Fire(g, actor);
            }
        }
    }
}
