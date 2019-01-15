using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameActorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void constructor_with_zero_health_should_throwArgumentOutOfRange()
        {
            var actorNotOk = new GameActor(0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void constructor_with_negative_health_should_throwArgumentOutOfRange()
        {
            var actorNotOk = new GameActor(-1);
        }
        [TestMethod]
        public void big_damage_should_zero_health()
        {
            var actor = new GameActor(100);
            actor.takeDamage(1000);
            Assert.AreEqual(actor.health, 0);
        }
        [TestMethod]
        public void big_heal_should_maximize_health()
        {
            var actor = new GameActor(100);
            actor.takeDamage(50);
            actor.heal(1000);
            Assert.AreEqual(actor.health, actor.baseHealth);
        }
        [TestMethod]
        public void actor_should_survive_damage()
        {
            var actor = new GameActor(100);
            actor.takeDamage(30);

            Assert.AreEqual(actor.health, 70);
            Assert.IsTrue(actor.alive);
        }
        [TestMethod]
        public void actor_should_die_from_damage()
        {
            var actor = new GameActor(100);
            actor.takeDamage(200);

            Assert.IsFalse(actor.alive);
        }
        [TestMethod]
        public void actor_with_no_weapon_should_do_no_damage()
        {
            var a = new GameActor(100);
            var b = new GameActor(100);

            a.dealDamage(b);

            Assert.AreEqual(b.baseHealth, b.health);
            Assert.IsTrue(b.alive);
        }
        [TestMethod]
        public void actor_with_weapon_should_deal_damage()
        {
            var a = new GameActor(100);
            var b = new GameActor(100);

            a.weapon = new GameWeapon("lunchbox", 50);

            a.dealDamage(b);
            Assert.AreEqual(b.health, b.baseHealth - a.weapon.damage);
            Assert.IsTrue(a.alive);
            Assert.IsTrue(b.alive);
        }
        [TestMethod]
        public void actor_with_weapon_should_kill()
        {
            var a = new GameActor(100);
            var b = new GameActor(100);

            a.weapon = new GameWeapon("deadly apple", 150);

            a.dealDamage(b);
            Assert.AreEqual(b.health, 0);
            Assert.IsFalse(b.alive);
        }
    }
}
