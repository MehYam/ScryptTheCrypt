﻿using System;
using System.Collections.Generic;
using ScryptTheCrypt.Actions;

namespace ScryptTheCrypt
{
    public sealed class GameActor
    {
        public readonly float baseHealth;
        public float Health { get; private set; }
        public GameWeapon Weapon { get; set; }
        public bool Alive { get { return Health > 0; } }
        public GameActor(float baseHealth = 100)
        {
            if (baseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHealth), "baseHealth must be greater than zero");
            }
            this.baseHealth = baseHealth;
            this.Health = baseHealth;
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

        // Action/Attribute system.  Actions are anonymous operations on GameActor.  Attributes
        // are variables store, retrieved, and shared between Actions (i.e. chosen target, buffs,
        // debuffs, etc).  
        //
        // This is an experimental design for the things I've always struggled to get from *Actor
        // classes.  All members of GameActor could literally be implemented as attributes.  Furthermore,
        // the way to do this with less code is to use ExpandoObject, but I'm not sure about any of
        // this yet, except that the way this is implemented now seems almost the worst of both 
        // static/dynamic worlds.
        //
        // Note that if performance of these were ever an issue, the Dictionary could be replaced with
        // an array, and the Attributes would work as indices.
        public enum Attribute { Target, Frozen, Sleeping };
        readonly List<IActorAction> actions = new List<IActorAction>();
        readonly Dictionary<Attribute, object> attributes = new Dictionary<Attribute, object>();

        public void AddAction(IActorAction a)
        {
            actions.Add(a);
        }
        public void SetAttribute(Attribute a, object o)
        {
            attributes[a] = o;
        }
        public object GetAttribute(Attribute a)
        {
            attributes.TryGetValue(a, out object retval);
            return retval;
        }
        public void ClearAttribute(Attribute a)
        {
            attributes.Remove(a);
        }
        public void DoActions(Game g)
        {
            foreach(var action in actions)
            {
                action.act(g, this);
            }
        }
    }
}
