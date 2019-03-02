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
            //KAI: will currently flunk test - call g.GetTargets, iterate them, attack for each one
            foreach (var target in g.GetTargets())
            {
                actor.Attack(target);
            }
        }
    }
}
