using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt.Actions
{
    public sealed class ActionChooseRandomTarget : IActorAction
    {
        readonly bool friendly;
        public ActionChooseRandomTarget(bool friendly = false)
        {
            this.friendly = friendly;
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

            // figure out if we're a player or mob
            bool bPlayer = g.players.Contains(actor);
            if (!bPlayer && !g.mobs.Contains(actor))
            {
                throw new ArgumentException("actor doesn't belong to game");
            }


        }
        public GameActor Choice { get; private set; }
    }
}
