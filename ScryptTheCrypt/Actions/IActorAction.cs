using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt.Actions
{
    public interface IActorAction
    {
        void act(Game g, GameActor actor);
    }
}
