using System;
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
            var game = new Game(1000);
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
            GameEvents.ReleaseAllListeners();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
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
            testGame.PreparePlayerToAttack();
            testGame.game.DoTurn();
            GameEvents.ReleaseAllListeners();

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
            testGame.PreparePlayerToAttack();
            Assert.IsTrue(fired);
            Assert.IsFalse(testGame.mob.Alive);
        }
    }
}
