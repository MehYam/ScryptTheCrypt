using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt.Actions
{
    public sealed class ActionAttack : IActorAction
    {
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
            var target = actor.GetAttribute(GameActor.Attribute.Target);
            if (target is GameActor)
            {
                actor.DealDamage((GameActor)target);
            }
        }
    }
}
