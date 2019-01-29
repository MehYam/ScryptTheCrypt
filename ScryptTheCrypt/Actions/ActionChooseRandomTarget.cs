using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt.Actions
{
    public sealed class ActionChooseRandomTarget : IActorAction
    {
        readonly GameBattle.ActorAlignment targetAlignment;
        public ActionChooseRandomTarget(GameBattle.ActorAlignment targetAlignment)
        {
            this.targetAlignment = targetAlignment;
        }
        public void act(GameBattle g, GameActor actor)
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
            GameActor target = null;
            switch(targetAlignment)
            {
                case GameBattle.ActorAlignment.Mob:
                    target = selectLiving(g.mobs);
                    break;
                case GameBattle.ActorAlignment.Player:
                    target = selectLiving(g.players);
                    break;
            }

            actor.SetAttribute(GameActor.Attribute.Target, target);
            if (target != null)
            {
                GameEvents.Instance.TargetChosen_Fire(g, actor);
            }
        }
    }
}
