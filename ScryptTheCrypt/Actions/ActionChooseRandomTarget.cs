using System;
using System.Collections.Generic;

namespace ScryptTheCrypt.Actions
{
    public sealed class ActionChooseRandomTarget : IActorAction
    {
        readonly GameActor.Alignment targetAlign;
        public ActionChooseRandomTarget(GameActor.Alignment targetAlign)
        {
            this.targetAlign = targetAlign;
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
            switch(targetAlign)
            {
                case GameActor.Alignment.Mob:
                    target = selectLiving(g.Mobs);
                    break;
                case GameActor.Alignment.Player:
                    target = selectLiving(g.Players);
                    break;
            }

            actor.Target = target;
        }
    }
}
