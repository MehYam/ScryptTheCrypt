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
            switch(targetAlignment)
            {
                case Game.ActorAlignment.Mob:
                    actor.SetAttribute(GameActor.Attribute.Target, g.mobs[g.rng.Next(0, g.mobs.Count - 1)]);
                    GameEvents.Instance.TargetChosen_Fire(g, actor);
                    break;
                case Game.ActorAlignment.Player:
                    actor.SetAttribute(GameActor.Attribute.Target, g.players[g.rng.Next(0, g.players.Count - 1)]);
                    GameEvents.Instance.TargetChosen_Fire(g, actor);
                    break;
            }
        }
    }
}
