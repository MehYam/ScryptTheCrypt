using System;

using KaiGameUtil;

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
        public event Action<Game, GameActor> ActorAdded = delegate { };
        public event Action<Game, GameActor> ActorRemoved = delegate { };
        public event Action<Game> RoundStart = delegate { };
        public event Action<Game> RoundEnd = delegate { };
        public event Action<Game, GameActor> ActorActionsStart = delegate { };
        public event Action<Game, GameActor> ActorActionsEnd = delegate { };
        public event Action<GameActor> ActorTargetedChange = delegate { };
        public event Action<GameActor, GameActor> AttackStart = delegate { };
        public event Action<GameActor, GameActor> AttackWillCrit = delegate { };
        public event Action<GameActor, GameActor> AttackEnd = delegate { };
        public event Action<GameActor, float, float> ActorHealthChange = delegate { };
        public event Action<GameActor, Point<int>> ActorDirectionChange = delegate { };
        public event Action<GameActor> Death = delegate { };

        public void ActorAdded_Fire(Game g, GameActor a) { ActorAdded(g, a); }
        public void ActorRemoved_Fire(Game g, GameActor a) { ActorRemoved(g, a); }
        public void RoundStart_Fire(Game g) { RoundStart(g); }
        public void RoundEnd_Fire(Game g) { RoundEnd(g); }
        public void ActorActionsStart_Fire(Game g, GameActor a) { ActorActionsStart(g, a); }
        public void ActorActionsEnd_Fire(Game g, GameActor a) { ActorActionsEnd(g, a); }
        public void ActorTargetedChange_Fire(GameActor a) { ActorTargetedChange(a); }
        public void ActorDirectionChange_Fire(GameActor a, Point<int> oldDir) { ActorDirectionChange(a, oldDir); }
        public void AttackStart_Fire(GameActor attacker, GameActor victim) { AttackStart(attacker, victim); }
        public void AttackWillCrit_Fire(GameActor attacker, GameActor victim) { AttackWillCrit(attacker, victim); }
        public void AttackEnd_Fire(GameActor attacker, GameActor victim) { AttackEnd(attacker, victim); }
        public void ActorHealthChange_Fire(GameActor a, float oldHealth, float newHealth) { ActorHealthChange(a, oldHealth, newHealth); }
        public void Death_Fire(GameActor a) { Death(a); }
    }
}
