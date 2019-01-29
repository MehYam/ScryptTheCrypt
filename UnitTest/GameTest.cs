using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void TurnWithoutPlayersShouldBeOk()
        {
            var game = new GameBattle(1000);
            game.DoTurn();
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
        [TestMethod]
        public void DeadMobsShouldReturnVictory()
        {
            testGame.KillMobs();

            Assert.AreEqual(testGame.game.GameProgress, GameBattle.Progress.PlayersWin);
        }
        [TestMethod]
        public void DeadPlayersShouldReturnLoss()
        {
            testGame.KillPlayers();

            Assert.AreEqual(testGame.game.GameProgress, GameBattle.Progress.MobsWin);
        }
        [TestMethod]
        public void AllDeadShouldReturnDraw()
        {
            testGame.KillPlayers();
            testGame.KillMobs();

            Assert.AreEqual(testGame.game.GameProgress, GameBattle.Progress.Draw);
        }
        [TestMethod]
        public void SurvivingPlayerAndMobShouldReturnGameInProgress()
        {
            Assert.AreEqual(testGame.game.GameProgress, GameBattle.Progress.InProgress);
        }
        [TestMethod]
        public void DeadActorsShouldGetNoTurn()
        {
            testGame.player.TakeDamage(testGame.player.Health);
            testGame.game.DoTurn();

            Assert.AreEqual(testGame.playerMockAction.timesCalled, 0);
            Assert.AreEqual(testGame.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoTurnShouldCallActionWithCorrectGame()
        {
            testGame.game.DoTurn();

            Assert.AreEqual(testGame.playerMockAction.gCalledWith, testGame.game);
            Assert.AreEqual(testGame.mobMockAction.gCalledWith, testGame.game);
        }
        [TestMethod]
        public void DoTurnShouldCallActionWithCorrectActor()
        {
            testGame.game.DoTurn();
            Assert.AreEqual(testGame.playerMockAction.aCalledWith, testGame.player);
            Assert.AreEqual(testGame.mobMockAction.aCalledWith, testGame.mob);
        }
        [TestMethod]
        public void DoTurnShouldCallActionOnce()
        {
            testGame.game.DoTurn();
            Assert.AreEqual(testGame.playerMockAction.timesCalled, 1);
            Assert.AreEqual(testGame.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoTurnShouldFireTurnEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.TurnStart += g => { startFired = true; };
            GameEvents.Instance.TurnEnd += g => { endFired = true; };
            testGame.game.DoTurn();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoTurnShouldFireActorActionsEvents()
        {
            bool fired = false;
            GameActor currentActor = null;
            GameEvents.Instance.ActorActionsStart += (g, a) =>
            {
                fired = true;
                Assert.AreEqual(g, testGame.game);
                Assert.IsNull(currentActor);
                currentActor = a;
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) =>
            {
                Assert.AreEqual(g, testGame.game);
                Assert.AreEqual(a, currentActor);
                currentActor = null;
            };

            testGame.game.DoTurn();

            Assert.IsTrue(fired);
            Assert.IsNull(currentActor);
        }
        // KAI: some of these are integration tests, and belong elsewhere.
        [TestMethod]
        public void DoTurnShouldFireAttackEvents()
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
            testGame.game.DoTurn();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoTurnShouldFireDeathEvents()
        {
            bool fired = false;
            GameEvents.Instance.Death += (g, a) =>
            {
                fired = true;
                Assert.AreEqual(a, testGame.mob);
            };
            testGame.ArmPlayer(testGame.mob.baseHealth);
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.DoTurn();
            Assert.IsTrue(fired);
            Assert.IsFalse(testGame.mob.Alive);
        }
        [TestMethod]
        public void DoTurnShouldFireTargetEvents()
        {
            bool fired = false;
            GameEvents.Instance.TargetChosen += (g, a) =>
            {
                fired = true;
            };
            testGame.AddChooseTargetToPlayer();
            testGame.game.DoTurn();
            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void EnumerateTurnShouldRunAllActions()
        {
            var game = new GameBattle();
            var player = new GameActor();
            var mob = new GameActor();

            var mock1 = new MockAction();
            var mock2 = new MockAction();
            player.AddAction(mock1);
            mob.AddAction(mock2);
            game.players.Add(player);
            game.mobs.Add(mob);

            var turns = game.EnumerateTurnActions();
            while(turns.MoveNext());

            Assert.AreEqual(1, mock1.timesCalled);
            Assert.AreEqual(1, mock2.timesCalled);
            Assert.IsTrue(mock1.orderCalledIn < mock2.orderCalledIn);
        }
        [TestMethod]
        public void EnumerateTurnShouldFireTurnEvents()
        {
            var game = new GameBattle();
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.TurnStart += g =>
            {
                Assert.IsFalse(endFired);
                startFired = true;
            };
            GameEvents.Instance.TurnEnd += g =>
            {
                Assert.IsTrue(startFired);
                endFired = true;
            };
            var turns = game.EnumerateTurnActions();
            while(turns.MoveNext());

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
    }
}
