using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameActorTest
    {
        [TestCleanup]
        public void TestCleanup()
        {
            GameEvents.ReleaseAllListeners();
        }
        [TestMethod]
        public void CloneShouldCreateACopy()
        {
            var actor = new GameActor("dummy", 111);
            actor.TakeDamage(actor.baseHealth / 2);

            var clone = actor.Clone();

            Assert.AreEqual(actor.name, clone.name);
            Assert.AreEqual(actor.baseHealth, clone.baseHealth);
            Assert.AreEqual(actor.Health, clone.Health);
        }
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
        public void ConstructorShouldCreateUniqueName()
        {
            var actor1 = new GameActor("wolf");
            var actor2 = new GameActor("wolf");

            Assert.AreEqual(actor1.name, actor2.name);
            Assert.AreNotEqual(actor1.uniqueName, actor2.uniqueName);
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

            GameEvents.Instance.ActorTurnStart += (g, a) => 
            {
                Assert.IsNull(currentActor);
                currentActor = a;

                Assert.AreEqual(testActor, a);
            };
            GameEvents.Instance.ActorTurnEnd += (g, a) => 
            {
                Assert.AreEqual(currentActor, a);
                Assert.AreEqual(testActor, a);
            };
            testActor.DoActions(null);

            Assert.AreEqual(currentActor, testActor);
        }
        [TestMethod]
        public void DamageShouldFireHealthChangeEvent()
        {
            GameActor actor = new GameActor();
            bool fired = false;
            GameEvents.Instance.ActorHealthChange += (a, oldHealth, newHealth) =>
            {
                fired = true;
                Assert.AreEqual(oldHealth, a.baseHealth);
                Assert.AreEqual(newHealth, actor.baseHealth / 2);
                Assert.AreEqual(newHealth, actor.Health);
            };
            actor.TakeDamage(actor.Health / 2);
            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void HealthChangeEventShouldFireAfterChange()
        {
            GameActor actor = new GameActor();
            bool fired = false;
            GameEvents.Instance.ActorHealthChange += (a, oldHealth, newHealth) =>
            {
                fired = true;
                Assert.AreEqual(newHealth, actor.Health);
            };
            actor.TakeDamage(actor.Health / 2);
            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void HealShouldFireHealthChangeEvent()
        {
            GameActor actor = new GameActor();
            bool fired = false;
            actor.TakeDamage(actor.baseHealth / 2);

            GameEvents.Instance.ActorHealthChange += (a, oldHealth, newHealth) =>
            {
                fired = true;
                Assert.AreEqual(oldHealth, a.baseHealth / 2);
                Assert.AreEqual(newHealth, a.baseHealth);
            };
            actor.Heal(actor.baseHealth);

            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void ZeroDamageShouldNotFireHealthChange()
        {
            GameActor actor = new GameActor();

            bool fired = false;
            GameEvents.Instance.ActorHealthChange += (a, oldHealth, newHealth) =>
            {
                fired = true;
            };
            actor.TakeDamage(0);
            Assert.IsFalse(fired);
        }
        [TestMethod]
        public void IneffectualHealShouldNotFireHealthChange()
        {
            GameActor actor = new GameActor();
            actor.TakeDamage(actor.baseHealth / 2);

            bool fired = false;
            GameEvents.Instance.ActorHealthChange += (a, oldHealth, newHealth) =>
            {
                fired = true;
            };
            actor.TakeDamage(0);
            Assert.IsFalse(fired);
        }
        [TestMethod]
        public void ActionEnumeratorShouldCallActions()
        {
            Game game = new Game();
            GameActor actor = new GameActor();

            MockAction action1 = new MockAction();
            MockAction action2 = new MockAction();

            actor.AddAction(action1);
            actor.AddAction(action2);

            var actions = actor.EnumerateActions(game);
            while (actions.MoveNext());

            Assert.AreEqual(action1.timesCalled, 1);
            Assert.AreEqual(action2.timesCalled, 1);
            Assert.IsTrue(action1.orderCalledIn < action2.orderCalledIn);
            Assert.AreEqual(action1.gCalledWith, game);
            Assert.AreEqual(action1.aCalledWith, actor);
        }
        [TestMethod]
        public void ActionEnumeratorShouldInvokeEvents()
        {
            Game game = new Game();
            GameActor actor = new GameActor();

            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.ActorTurnStart += (g, a) =>
            {
                startFired = true;
            };
            GameEvents.Instance.ActorTurnEnd += (g, a) =>
            {
                endFired = true;
            };
            var actions = actor.EnumerateActions(game);
            while (actions.MoveNext());

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
    }
}
