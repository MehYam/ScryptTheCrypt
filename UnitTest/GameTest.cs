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
    }
}
