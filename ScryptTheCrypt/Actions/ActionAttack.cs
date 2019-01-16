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
            var target = actor.GetAttribute(GameActor.Attribute.Target) as GameActor;
            if (target != null)
            {
                bool wasAlive = target.Alive;

                GameEvents.Instance.AttackStart_Fire(g, actor, target);
                actor.DealDamage((GameActor)target);
                GameEvents.Instance.AttackEnd_Fire(g, actor, target);

                //KAI: not quite clear who should be firing the events for what;  for example,
                // shouldn't the actor really fire this?  It doesn't now mainly because the event
                // passes along the Game instance.  "Might should change this."
                if (wasAlive && !target.Alive)
                {
                    GameEvents.Instance.Death_Fire(g, target);
                }
            }
        }
    }
}
