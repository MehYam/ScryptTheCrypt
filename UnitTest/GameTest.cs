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
        TestGameWithActors testData = null;
        [TestInitialize]
        public void TestInitialize()
        {
            testData = new TestGameWithActors();
        }
        [TestCleanup]
        public void TestCleanup()
        {
            testData = null;
            GameEvents.ReleaseAllListeners();
        }
        [TestMethod]
        public void NumRoundsShouldStartZero()
        {
            Assert.AreEqual(0, testData.game.NumRounds);
        }
        [TestMethod]
        public void PlayRoundShouldIncrementNumRounds()
        {
            testData.game.PlayRound();

            Assert.AreEqual(1, testData.game.NumRounds);
        }
        [TestMethod]
        public void EnumerateRoundShouldIncrementNumRounds()
        {
            var actions = testData.game.EnumerateRound();
            Assert.AreEqual(0, testData.game.NumRounds);

            while (actions.MoveNext())
            {
                Assert.AreEqual(1, testData.game.NumRounds);
            }

            Assert.AreEqual(1, testData.game.NumRounds);
        }
        [TestMethod]
        public void RoundStartShouldFireAfterNumRoundsSet()
        {
            GameEvents.Instance.RoundStart += g =>
            {
                Assert.AreEqual(1, testData.game.NumRounds);
            };
            testData.game.PlayRound();
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
            testData.game.Players[0] = null;
        }
        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public void MobListShouldBeReadOnly()
        {
            testData.game.Mobs[0] = null;
        }
        [TestMethod]
        public void ClearActorsShouldEmptyActorList()
        {
            Assert.IsTrue(testData.game.Players.Count > 0);
            Assert.IsTrue(testData.game.Mobs.Count > 0);

            testData.game.ClearActors(Game.ActorAlignment.Mob);
            Assert.IsTrue(testData.game.Players.Count > 0);
            Assert.AreEqual(0, testData.game.Mobs.Count);

            testData.game.ClearActors(Game.ActorAlignment.Player);
            Assert.AreEqual(0, testData.game.Players.Count);
        }
        [TestMethod]
        public void DeadMobsShouldReturnVictory()
        {
            testData.KillMobs();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.PlayersWin);
        }
        [TestMethod]
        public void DeadPlayersShouldReturnLoss()
        {
            testData.KillPlayers();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.MobsWin);
        }
        [TestMethod]
        public void AllDeadShouldReturnDraw()
        {
            testData.KillPlayers();
            testData.KillMobs();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.Draw);
        }
        [TestMethod]
        public void SurvivingPlayerAndMobShouldReturnGameInProgress()
        {
            Assert.AreEqual(testData.game.GameProgress, Game.Progress.InProgress);
        }
        [TestMethod]
        public void DeadActorsShouldGetNoTurn()
        {
            testData.player.TakeDamage(testData.player.Health);
            testData.game.PlayRound();

            Assert.AreEqual(testData.playerMockAction.timesCalled, 0);
            Assert.AreEqual(testData.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectGame()
        {
            testData.game.PlayRound();

            Assert.AreEqual(testData.playerMockAction.gCalledWith, testData.game);
            Assert.AreEqual(testData.mobMockAction.gCalledWith, testData.game);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectActor()
        {
            testData.game.PlayRound();
            Assert.AreEqual(testData.playerMockAction.aCalledWith, testData.player);
            Assert.AreEqual(testData.mobMockAction.aCalledWith, testData.mob);
        }
        [TestMethod]
        public void DoRoundShouldCallActionOnce()
        {
            testData.game.PlayRound();
            Assert.AreEqual(testData.playerMockAction.timesCalled, 1);
            Assert.AreEqual(testData.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldFireTurnEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.RoundStart += g => { startFired = true; };
            GameEvents.Instance.RoundEnd += g => { endFired = true; };
            testData.game.PlayRound();

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
                Assert.AreEqual(g, testData.game);
                Assert.IsNull(currentActor);
                currentActor = a;
            };
            GameEvents.Instance.ActorTurnEnd += (g, a) =>
            {
                Assert.AreEqual(g, testData.game);
                Assert.AreEqual(a, currentActor);
                currentActor = null;
            };

            testData.game.PlayRound();

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
                Assert.AreEqual(a, testData.player);
                Assert.AreEqual(b, testData.mob);
                startFired = true;
            };
            GameEvents.Instance.AttackEnd += (g, a, b) =>
            {
                Assert.AreEqual(a, testData.player);
                Assert.AreEqual(b, testData.mob);
                endFired = true;
            };
            testData.ArmPlayer();
            testData.AddTargetAndAttackToPlayer();
            testData.game.PlayRound();

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
                Assert.AreEqual(a, testData.mob);
            };
            testData.ArmPlayer(testData.mob.baseHealth);
            testData.AddTargetAndAttackToPlayer();
            testData.game.PlayRound();
            Assert.IsTrue(fired);
            Assert.IsFalse(testData.mob.Alive);
        }
        [TestMethod]
        public void DoRoundShouldFireTargetEvents()
        {
            bool fired = false;
            GameEvents.Instance.TargetChosen += (g, a) =>
            {
                fired = true;
            };
            testData.AddChooseTargetToPlayer();
            testData.game.PlayRound();
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

            var turns = game.EnumerateRound();
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
            var turns = game.EnumerateRound();
            while(turns.MoveNext());

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
    }
}
