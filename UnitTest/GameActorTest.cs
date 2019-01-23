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
        public void ConstructorWithZeroHealthShouldThrow()
        {
            var actorNotOk = new GameActor("zombie", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorWithNegativeHealthShouldThrow()
        {
            var actorNotOk = new GameActor("vampire", -1);
        }
        [TestMethod]
        public void BigDamageShouldZeroHealth()
        {
            var actor = new GameActor("doomed", 100);
            actor.TakeDamage(1000);
            Assert.AreEqual(actor.Health, 0);
        }
        [TestMethod]
        public void BigHealShouldMaxHealth()
        {
            var actor = new GameActor("lucky", 100);
            actor.TakeDamage(50);
            actor.Heal(1000);
            Assert.AreEqual(actor.Health, actor.baseHealth);
        }
        [TestMethod]
        public void ActorCanSurviveDamage()
        {
            var actor = new GameActor("hurt", 100);
            actor.TakeDamage(30);

            Assert.AreEqual(actor.Health, 70);
            Assert.IsTrue(actor.Alive);
        }
        [TestMethod]
        public void ActorCanDieFromDamage()
        {
            var actor = new GameActor("why me", 100);
            actor.TakeDamage(200);

            Assert.IsFalse(actor.Alive);
        }
        [TestMethod]
        public void WeaponlessActorDoesNoDamage()
        {
            var a = new GameActor();
            var b = new GameActor();

            a.DealDamage(b);

            Assert.AreEqual(b.baseHealth, b.Health);
            Assert.IsTrue(b.Alive);
        }
        [TestMethod]
        public void WeaponfulActorDoesDamage()
        {
            var a = new GameActor();
            var b = new GameActor();

            a.Weapon = new GameWeapon("lunchbox", b.baseHealth - 1);

            a.DealDamage(b);
            Assert.AreEqual(b.Health, b.baseHealth - a.Weapon.damage);
            Assert.IsTrue(a.Alive);
            Assert.IsTrue(b.Alive);
        }
        [TestMethod]
        public void WeaponfulActorCanKill()
        {
            var a = new GameActor();
            var b = new GameActor();

            a.Weapon = new GameWeapon("deadly apple", b.baseHealth);

            a.DealDamage(b);
            Assert.AreEqual(b.Health, 0);
            Assert.IsFalse(b.Alive);
        }
        [TestMethod]
        public void DoActionsShouldFireActorActionsEvents()
        {
            GameActor currentActor = null;
            GameActor testActor = new GameActor();

            GameEvents.Instance.ActorActionsStart += (g, a) => 
            {
                Assert.IsNull(currentActor);
                currentActor = a;

                Assert.AreEqual(testActor, a);
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) => 
            {
                Assert.AreEqual(currentActor, a);
                Assert.AreEqual(testActor, a);
            };
            testActor.DoActions(null);

            Assert.AreEqual(currentActor, testActor);
            GameEvents.ReleaseAllListeners();
        }
    }
}
