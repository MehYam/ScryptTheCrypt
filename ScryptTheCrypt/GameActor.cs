using System;
using System.Collections;
using System.Collections.Generic;

using ScryptTheCrypt.Actions;

namespace ScryptTheCrypt
{
    public sealed class GameActor
    {
        public enum Alignment { Player, Mob };
        static private int s_instance = 0;

        public readonly int id;
        public readonly Alignment align;
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
                    bool wasAlive = Alive;
                    float oldHealth = _health;

                    _health = value;

                    GameEvents.Instance.ActorHealthChange_Fire(this, oldHealth, _health);
                    if (wasAlive && !Alive)
                    {
                        GameEvents.Instance.Death_Fire(this);
                    }
                }
            }
        }
        public GameWeapon Weapon { get; set; }
        public bool Alive { get { return Health > 0; } }

        private bool _targeted;
        public bool Targeted
        {
            get { return _targeted; }
            set
            {
                _targeted = value;
                GameEvents.Instance.ActorTargetedChange_Fire(this);
            }
        }
        public bool Frozen { get; set; }
        public bool Sleeping { get; set; }

        public GameActor(Alignment align = Alignment.Player, string name = "anon", float baseHealth = 100)
        {
            id = ++s_instance;
            if (baseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHealth), "baseHealth must be greater than zero");
            }
            this.align = align;
            this.name = name;
            this.uniqueName = $"{name}:{id.ToString("D4")}";
            this.baseHealth = baseHealth;
            _health = baseHealth;
        }
        public GameActor(Alignment align, string name, float baseHealth, GameWeapon weapon) : this(align, name, baseHealth)
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
            sb.Append($" targeted: {Targeted} frozen: {Frozen} sleeping: {Sleeping} ");
            var weaponText = Weapon != null ? Weapon.ToString() : "none";
            return $"GameActor '{uniqueName}', health {Health}/{baseHealth}, weapon {weaponText}, {align}, attrs {sb}";
        }
        public void TakeDamage(float d)
        {
            Health = Math.Max(0, Health - d);
        }
        public void Heal(float h)
        {
            Health = Math.Min(baseHealth, Health + h);
        }
        public void Attack(GameActor other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other), "other actor is null");
            if (Weapon != null)
            {
                GameEvents.Instance.AttackStart_Fire(this, other);
                other.TakeDamage(Weapon.damage);
                GameEvents.Instance.AttackEnd_Fire(this, other);
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
        private Jint.Engine jint;
        public void SetScrypt(string js)
        {
            jint = new Jint.Engine();
            jint.SetValue("Alignment_Mob", Alignment.Mob);
            jint.SetValue("Alignment_Player", Alignment.Player);
            jint.Execute(js);
        }
        public void SetScryptLogger(Delegate logger)
        {
            jint.SetValue("log", logger);
        }
        public void RunScrypt(Game g)
        {
            GameEvents.Instance.ActorActionsStart_Fire(g, this);
            if (jint != null)
            {
                jint.Invoke("actorActions", g, this);
            }
            GameEvents.Instance.ActorActionsEnd_Fire(g, this);
        }
    }
}
