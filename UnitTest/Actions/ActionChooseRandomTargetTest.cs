using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest.Actions
{
    [TestClass]
    public class ActionChooseRandomTargetTest
    {
        [TestCleanup]
        public void TestCleanup()
        {
            GameEvents.ReleaseAllListeners();
        }
        [TestMethod]
        public void PseudoRandomPlayerTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.playerAlice.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Player));
            testGame.game.PlayRound();

            var targets = testGame.game.GetTargets();
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].align == GameActor.Alignment.Player);
        }
        [TestMethod]
        public void PseudoRandomMobTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.playerAlice.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Mob));
            testGame.game.PlayRound();

            var targets = testGame.game.GetTargets();
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].align == GameActor.Alignment.Mob);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallingActWithNoGameThrows()
        {
            var action = new ActionChooseRandomTarget(GameActor.Alignment.Player);
            action.act(null, new GameActor());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallingActWithNoActorThrows()
        {
            var action = new ActionChooseRandomTarget(GameActor.Alignment.Player);
            action.act(new Game(), null);
        }
        [TestMethod]
        public void DeadActorsShouldNotBeTargeted()
        {
            var game = new Game();
            var deadHorse = new GameActor(GameActor.Alignment.Mob);
            deadHorse.TakeDamage(deadHorse.Health);

            game.AddActor(deadHorse);

            var action = new ActionChooseRandomTarget(GameActor.Alignment.Mob);
            var chooser = new GameActor(GameActor.Alignment.Player);

            game.AddActor(chooser);
            action.act(game, chooser);

            Assert.AreEqual(0, game.GetTargets().Count);
        }
    }
}
