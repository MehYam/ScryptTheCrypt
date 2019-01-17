using System;
using System.Collections.Generic;
using System.Text;

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
            GameActor selectLiving(List<GameActor> actors)
            {
                var nondead = actors.FindAll(a => a.Alive);
                return nondead.Count > 0 ? nondead[g.rng.Next(0, nondead.Count - 1)] : null;
            }
            switch(targetAlignment)
            {
                case Game.ActorAlignment.Mob:
                    actor.SetAttribute(GameActor.Attribute.Target, selectLiving(g.mobs));
                    GameEvents.Instance.TargetChosen_Fire(g, actor);
                    break;
                case Game.ActorAlignment.Player:
                    actor.SetAttribute(GameActor.Attribute.Target, selectLiving(g.players));
                    GameEvents.Instance.TargetChosen_Fire(g, actor);
                    break;
            }
        }
    }
}
