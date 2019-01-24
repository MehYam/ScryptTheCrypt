using System;
using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt
{
    // NOTE: a while back, WeakEventManager was introduced to solve the same sorts of problems this
    // does.  I like this better, it's a little simpler, and uses the built-in += event subscription mechanism
    public sealed class GameEvents
    {
        static GameEvents _singleton = null;
        GameEvents() { } // hide construction

        static public GameEvents Instance
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new GameEvents();
                }
                return _singleton;
            }
        }
        static public void ReleaseAllListeners()
        {
            _singleton = null;
        }
        //KAI: should remove the Game reference from all of these - we don't really need them everywhere, and maybe not
        // anywhere
        public event Action<Game> TurnStart = delegate { };
        public event Action<Game> TurnEnd = delegate { };
        public event Action<Game, GameActor> ActorActionsStart = delegate { };
        public event Action<Game, GameActor> ActorActionsEnd = delegate { };
        public event Action<Game, GameActor> TargetChosen = delegate { };
        public event Action<Game, GameActor, GameActor> AttackStart = delegate { };
        public event Action<Game, GameActor, GameActor> AttackEnd = delegate { };
        public event Action<GameActor, float, float> ActorHealthChange = delegate { };
        public event Action<Game, GameActor> Death = delegate { };

        //KAI: it may be wrong/unnecessary to be passing back the Game instance everywhere, but it is convenient
        public void TurnStart_Fire(Game g) { TurnStart(g); }
        public void TurnEnd_Fire(Game g) { TurnEnd(g); }
        public void ActorActionsStart_Fire(Game g, GameActor a) { ActorActionsStart(g, a); }
        public void ActorActionsEnd_Fire(Game g, GameActor a) { ActorActionsEnd(g, a); }
        public void TargetChosen_Fire(Game g, GameActor a) { TargetChosen(g, a); }
        public void AttackStart_Fire(Game g, GameActor attacker, GameActor victim) { AttackStart(g, attacker, victim); }
        public void AttackEnd_Fire(Game g, GameActor attacker, GameActor victim) { AttackEnd(g, attacker, victim); }
        public void ActorHealthChange_Fire(GameActor a, float oldHealth, float newHealth) { ActorHealthChange(a, oldHealth, newHealth); }
        public void Death_Fire(Game g, GameActor a) { Death(g, a); }
    }
}
