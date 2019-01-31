using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void TurnWithoutPlayersShouldBeOk()
        {
            var game = new Game(1000);
            game.PlayRound();
        }
        TestGameWithActors testGame = null;
        [TestInitialize]
        public void TestInitialize()
        {
            testGame = new TestGameWithActors();
        }
        [TestCleanup]
        public void TestCleanup()
        {
            testGame = null;
            GameEvents.ReleaseAllListeners();
        }
        private void TestAddActor(Game.ActorAlignment align)
        {
            var game = new Game();
            var actor1 = new GameActor("one");
            var actor2 = new GameActor("two");
            game.AddActor(actor1, align);
            game.AddActor(actor2, align);

            var coll = align == Game.ActorAlignment.Player ? game.Players : game.Mobs;
            Assert.AreEqual(coll[0], actor1);
            Assert.AreEqual(coll[1], actor2);
        }
        [TestMethod]
        public void AddPlayerShouldAddActorsInOrder()
        {
            TestAddActor(Game.ActorAlignment.Player);
        }
        [TestMethod]
        public void AddMobShouldAddActorsInOrder()
        {
            TestAddActor(Game.ActorAlignment.Mob);
        }
        private void TestAddActorEvent(Game.ActorAlignment align)
        {
            var game = new Game();
            var actor = new GameActor();
            int called = 0;
            GameEvents.Instance.ActorAdded += (g, a, al) =>
            {
                ++called;
                Assert.AreEqual(game, g);
                Assert.AreEqual(a, actor);
                Assert.AreEqual(al, align);
            };
            game.AddActor(actor, align);
            Assert.AreEqual(1, called);
        }
        [TestMethod]
        public void AddActorShouldFireEvent()
        {
            TestAddActorEvent(Game.ActorAlignment.Player);
        }
        [TestMethod]
        public void AddMobShouldFireEvent()
        {
            TestAddActorEvent(Game.ActorAlignment.Mob);
        }
        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public void PlayerListShouldBeReadOnly()
        {
            testGame.game.Players[0] = null;
        }
        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public void MobListShouldBeReadOnly()
        {
            testGame.game.Mobs[0] = null;
        }
        [TestMethod]
        public void DeadMobsShouldReturnVictory()
        {
            testGame.KillMobs();

            Assert.AreEqual(testGame.game.GameProgress, Game.Progress.PlayersWin);
        }
        [TestMethod]
        public void DeadPlayersShouldReturnLoss()
        {
            testGame.KillPlayers();

            Assert.AreEqual(testGame.game.GameProgress, Game.Progress.MobsWin);
        }
        [TestMethod]
        public void AllDeadShouldReturnDraw()
        {
            testGame.KillPlayers();
            testGame.KillMobs();

            Assert.AreEqual(testGame.game.GameProgress, Game.Progress.Draw);
        }
        [TestMethod]
        public void SurvivingPlayerAndMobShouldReturnGameInProgress()
        {
            Assert.AreEqual(testGame.game.GameProgress, Game.Progress.InProgress);
        }
        [TestMethod]
        public void DeadActorsShouldGetNoTurn()
        {
            testGame.player.TakeDamage(testGame.player.Health);
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.playerMockAction.timesCalled, 0);
            Assert.AreEqual(testGame.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectGame()
        {
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.playerMockAction.gCalledWith, testGame.game);
            Assert.AreEqual(testGame.mobMockAction.gCalledWith, testGame.game);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectActor()
        {
            testGame.game.PlayRound();
            Assert.AreEqual(testGame.playerMockAction.aCalledWith, testGame.player);
            Assert.AreEqual(testGame.mobMockAction.aCalledWith, testGame.mob);
        }
        [TestMethod]
        public void DoRoundShouldCallActionOnce()
        {
            testGame.game.PlayRound();
            Assert.AreEqual(testGame.playerMockAction.timesCalled, 1);
            Assert.AreEqual(testGame.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldFireTurnEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.RoundStart += g => { startFired = true; };
            GameEvents.Instance.RoundEnd += g => { endFired = true; };
            testGame.game.PlayRound();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoRoundShouldFireActorActionsEvents()
        {
            bool fired = false;
            GameActor currentActor = null;
            GameEvents.Instance.ActorTurnStart += (g, a) =>
            {
                fired = true;
                Assert.AreEqual(g, testGame.game);
                Assert.IsNull(currentActor);
                currentActor = a;
            };
            GameEvents.Instance.ActorTurnEnd += (g, a) =>
            {
                Assert.AreEqual(g, testGame.game);
                Assert.AreEqual(a, currentActor);
                currentActor = null;
            };

            testGame.game.PlayRound();

            Assert.IsTrue(fired);
            Assert.IsNull(currentActor);
        }
        // KAI: we're mixing unit and integration tests, look into how to manage that
        [TestMethod]
        public void DoRoundShouldFireAttackEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.AttackStart += (g, a, b) =>
            {
                Assert.AreEqual(a, testGame.player);
                Assert.AreEqual(b, testGame.mob);
                startFired = true;
            };
            GameEvents.Instance.AttackEnd += (g, a, b) =>
            {
                Assert.AreEqual(a, testGame.player);
                Assert.AreEqual(b, testGame.mob);
                endFired = true;
            };
            testGame.ArmPlayer();
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.PlayRound();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoRoundShouldFireDeathEvents()
        {
            bool fired = false;
            GameEvents.Instance.Death += (g, a) =>
            {
                fired = true;
                Assert.AreEqual(a, testGame.mob);
            };
            testGame.ArmPlayer(testGame.mob.baseHealth);
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.PlayRound();
            Assert.IsTrue(fired);
            Assert.IsFalse(testGame.mob.Alive);
        }
        [TestMethod]
        public void DoRoundShouldFireTargetEvents()
        {
            bool fired = false;
            GameEvents.Instance.TargetChosen += (g, a) =>
            {
                fired = true;
            };
            testGame.AddChooseTargetToPlayer();
            testGame.game.PlayRound();
            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void EnumerateTurnShouldRunAllActions()
        {
            var game = new Game();
            var player = new GameActor();
            var mob = new GameActor();

            var mock1 = new MockAction();
            var mock2 = new MockAction();
            player.AddAction(mock1);
            mob.AddAction(mock2);
            game.AddActor(player, Game.ActorAlignment.Player);
            game.AddActor(mob, Game.ActorAlignment.Mob);

            var turns = game.EnumerateRoundActions();
            while(turns.MoveNext());

            Assert.AreEqual(1, mock1.timesCalled);
            Assert.AreEqual(1, mock2.timesCalled);
            Assert.IsTrue(mock1.orderCalledIn < mock2.orderCalledIn);
        }
        [TestMethod]
        public void EnumerateTurnShouldFireTurnEvents()
        {
            var game = new Game();
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.RoundStart += g =>
            {
                Assert.IsFalse(endFired);
                startFired = true;
            };
            GameEvents.Instance.RoundEnd += g =>
            {
                Assert.IsTrue(startFired);
                endFired = true;
            };
            var turns = game.EnumerateRoundActions();
            while(turns.MoveNext());

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
    }
}
