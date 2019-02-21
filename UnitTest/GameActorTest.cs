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
            var actor = new GameActor(GameActor.Alignment.Player, "dummy", 111);
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
            var actorNotOk = new GameActor(GameActor.Alignment.Mob, "zombie", 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorWithNegativeHealthShouldThrow()
        {
            var actorNotOk = new GameActor(GameActor.Alignment.Mob, "vampire", -1);
        }
        [TestMethod]
        public void ConstructorShouldCreateUniqueName()
        {
            var actor1 = new GameActor(GameActor.Alignment.Mob, "wolf");
            var actor2 = new GameActor(GameActor.Alignment.Mob, "wolf");

            Assert.AreEqual(actor1.name, actor2.name);
            Assert.AreNotEqual(actor1.uniqueName, actor2.uniqueName);
        }
        [TestMethod]
        public void BigDamageShouldZeroHealth()
        {
            var actor = new GameActor(GameActor.Alignment.Mob, "doomed", 100);
            actor.TakeDamage(1000);
            Assert.AreEqual(actor.Health, 0);
        }
        [TestMethod]
        public void BigHealShouldMaxHealth()
        {
            var actor = new GameActor(GameActor.Alignment.Mob, "lucky", 100);
            actor.TakeDamage(50);
            actor.Heal(1000);
            Assert.AreEqual(actor.Health, actor.baseHealth);
        }
        [TestMethod]
        public void ActorCanSurviveDamage()
        {
            var actor = new GameActor(GameActor.Alignment.Mob, "hurt", 100);
            actor.TakeDamage(30);

            Assert.AreEqual(actor.Health, 70);
            Assert.IsTrue(actor.Alive);
        }
        [TestMethod]
        public void ActorCanDieFromDamage()
        {
            var actor = new GameActor(GameActor.Alignment.Mob, "why me", 100);
            actor.TakeDamage(actor.baseHealth);

            Assert.IsFalse(actor.Alive);
        }
        [TestMethod]
        public void DeathShouldFireEvent()
        {
            bool didFire = false;
            var a = new GameActor(GameActor.Alignment.Mob, "again really?", 100);
            GameEvents.Instance.Death += deceased =>
            {
                Assert.AreEqual(a, deceased);
                didFire = true;
            };

            a.TakeDamage(a.baseHealth);
            Assert.IsTrue(didFire);
        }
        [TestMethod]
        public void WeaponlessActorDoesNoDamage()
        {
            var a = new GameActor();
            var b = new GameActor();

            a.Attack(b);

            Assert.AreEqual(b.baseHealth, b.Health);
            Assert.IsTrue(b.Alive);
        }
        [TestMethod]
        public void WeaponfulActorDoesDamage()
        {
            var a = new GameActor();
            var b = new GameActor();

            a.Weapon = new GameWeapon("lunchbox", b.baseHealth - 1);

            a.Attack(b);
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

            a.Attack(b);
            Assert.AreEqual(b.Health, 0);
            Assert.IsFalse(b.Alive);
        }
        [TestMethod]
        public void AttackShouldFireEvents()
        {
            var a = new GameActor();
            var b = new GameActor();
            int progress = 0;
            GameEvents.Instance.AttackStart += (actorA, actorB) =>
            {
                Assert.AreEqual(0, progress++);
                Assert.AreEqual(a, actorA);
                Assert.AreEqual(b, actorB);
            };
            GameEvents.Instance.AttackEnd += (actorA, actorB) =>
            {
                Assert.AreEqual(1, progress++);
                Assert.AreEqual(a, actorA);
                Assert.AreEqual(b, actorB);
            };

            a.Weapon = new GameWeapon("deadly apple", b.baseHealth / 2);
            a.Attack(b);

            Assert.AreEqual(2, progress);
        }
        [TestMethod]
        public void DoActionsShouldFireActorActionsEvents()
        {
            GameActor currentActor = null;
            GameActor testActor = new GameActor();
            bool endFired = false;

            GameEvents.Instance.ActorActionsStart += (g, a) => 
            {
                Assert.IsFalse(endFired);
                Assert.IsNull(currentActor);
                currentActor = a;

                Assert.AreEqual(testActor, a);
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) => 
            {
                endFired = true;
                Assert.AreEqual(currentActor, a);
                Assert.AreEqual(testActor, a);
            };
            testActor.DoActions(null);

            Assert.AreEqual(currentActor, testActor);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void RunScryptShouldFireActorActionsEvents()
        {
            GameActor currentActor = null;
            GameActor testActor = new GameActor();
            bool endFired = false;

            GameEvents.Instance.ActorActionsStart += (g, a) => 
            {
                Assert.IsFalse(endFired);
                Assert.IsNull(currentActor);
                currentActor = a;

                Assert.AreEqual(testActor, a);
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) => 
            {
                endFired = true;
                Assert.AreEqual(currentActor, a);
                Assert.AreEqual(testActor, a);
            };
            testActor.RunScrypt(null);

            Assert.AreEqual(currentActor, testActor);
            Assert.IsTrue(endFired);
        }
        [ExpectedException(typeof(Jint.Runtime.JavaScriptException))]
        [TestMethod]
        public void SetScryptShouldThrowOnBadScrypt()
        {
            var actor = new GameActor();
            actor.SetScrypt("ur_nofun()");
        }
        [TestMethod]
        public void ScryptCanAccessGameActor()
        {
            var testGame = new Game();
            var testActor = new GameActor();

            testActor.SetScrypt(@"
            function actorActions(game, actor) {
                actor.TakeDamage(5);
            }
            ");

            testActor.RunScrypt(testGame);

            Assert.AreEqual(testActor.baseHealth - 5, testActor.Health);
        }
        [TestMethod]
        public void ScryptCanAccessAlignmentEnum()
        {
            var testActor = new GameActor();
            testActor.SetScrypt(@"
                if (Alignment_Mob == undefined || Alignment_Player == undefined) {
                    throw 'GameActor enums do not exist?';
                }
            ");
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
            GameEvents.Instance.ActorActionsStart += (g, a) =>
            {
                startFired = true;
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) =>
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
