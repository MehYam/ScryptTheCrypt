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
        public event Action<Game> TurnStart = delegate { };
        public event Action<Game> TurnEnd = delegate { };
        public event Action<Game, GameActor, GameActor> AttackStart = delegate { };
        public event Action<Game, GameActor, GameActor> AttackEnd = delegate { };

        public void TurnStart_Fire(Game g) { TurnStart(g); }
        public void TurnEnd_Fire(Game g) { TurnEnd(g); }
        public void AttackStart_Fire(Game g, GameActor attacker, GameActor victim) { AttackStart(g, attacker, victim); }
        public void AttackEnd_Fire(Game g, GameActor attacker, GameActor victim) { AttackEnd(g, attacker, victim); }
    }
}
