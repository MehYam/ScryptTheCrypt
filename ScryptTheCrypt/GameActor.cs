using System;
using System.Collections;
using System.Collections.Generic;

using ScryptTheCrypt.Actions;

namespace ScryptTheCrypt
{
    public sealed class GameActor
    {
        static GameActor()
        {
            MoonSharp.Interpreter.UserData.RegisterType<GameActor>();
        }
        static private int s_instance = 0;

        public readonly int id;
        public readonly string name;
        public readonly string uniqueName; // for convenience
        public readonly float baseHealth;

        private float _health;
        public float Health
        {
            get
            {
                return _health;
            }
            private set
            {
                if (_health != value)
                {
                    float oldHealth = _health;
                    _health = value;

                    GameEvents.Instance.ActorHealthChange_Fire(this, oldHealth, _health);
                }
            }
        }
        public GameWeapon Weapon { get; set; }
        public bool Alive { get { return Health > 0; } }

        // other secondary attributes - originally this was implemented as a dictionary, leaning towards
        // allowing clients of GameActor to associate whatever attributes they wanted with actors.  While
        // that would still be cool, for now we're just pounding them out as primitive types, since that
        // works a little easier with lua.
        private GameActor _target;
        public GameActor Target
        {
            get { return _target; }
            set
            {
                _target = value;
                if (_target != null)
                {
                    GameEvents.Instance.TargetSelected_Fire(this);
                }
            }
        }
        public bool Frozen { get; set; }
        public bool Sleeping { get; set; }

        public GameActor(string name = "anon", float baseHealth = 100)
        {
            id = ++s_instance;
            if (baseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHealth), "baseHealth must be greater than zero");
            }
            this.name = name;
            this.uniqueName = $"{name}:{id.ToString("D4")}";
            this.baseHealth = baseHealth;
            _health = baseHealth;
        }
        public GameActor(string name, float baseHealth, GameWeapon weapon) : this(name, baseHealth)
        {
            this.Weapon = weapon;
        }
        public GameActor Clone()
        {
            return this.MemberwiseClone() as GameActor;
        }
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($" target: {(Target != null ? Target.uniqueName : "none")} frozen: {Frozen} sleeping: {Sleeping} ");
            var weaponText = Weapon != null ? Weapon.ToString() : "none";
            return $"GameActor '{name}':{id.ToString("D4")}, health {Health}/{baseHealth}, weapon {weaponText}, attrs {sb}";
        }
        public void TakeDamage(float d)
        {
            Health = Math.Max(0, Health - d);
        }
        public void Heal(float h)
        {
            Health = Math.Min(baseHealth, Health + h);
        }
        public void DealDamage(GameActor other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other), "other actor is null");
            if (Weapon != null)
            {
                other.TakeDamage(Weapon.damage);
            }
        }
        readonly List<IActorAction> actions = new List<IActorAction>();
        public void AddAction(IActorAction a)
        {
            actions.Add(a);
        }
        public void DoActions(Game g)
        {
            GameEvents.Instance.ActorActionsStart_Fire(g, this);
            foreach(var action in actions)
            {
                action.act(g, this);
            }
            GameEvents.Instance.ActorActionsEnd_Fire(g, this);
        }
        public IEnumerator EnumerateActions(Game g)
        {
            GameEvents.Instance.ActorActionsStart_Fire(g, this);
            foreach (var action in actions)
            {
                action.act(g, this);
                yield return null;
            }
            GameEvents.Instance.ActorActionsEnd_Fire(g, this);
            yield break;
        }
        private MoonSharp.Interpreter.Script scrypt;
        public void SetScrypt(string lua)
        {
            scrypt = new MoonSharp.Interpreter.Script();
            scrypt.DoString(lua);
        }
        public void RunScrypt(Game g)
        {
            GameEvents.Instance.ActorActionsStart_Fire(g, this);
            if (scrypt != null)
            {
                scrypt.Call(scrypt.Globals["scrypt"], g, this);
            }
            GameEvents.Instance.ActorActionsEnd_Fire(g, this);
        }
    }
}
