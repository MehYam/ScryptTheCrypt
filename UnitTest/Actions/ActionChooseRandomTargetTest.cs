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

            Assert.IsNotNull(testGame.playerAlice.Target);
            Assert.IsTrue(testGame.game.Players.Contains(testGame.playerAlice.Target));
        }
        [TestMethod]
        public void PseudoRandomMobTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.playerAlice.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Mob));
            testGame.game.PlayRound();

            Assert.IsNotNull(testGame.playerAlice.Target);
            Assert.IsTrue(testGame.game.Mobs.Contains(testGame.playerAlice.Target));
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

            Assert.IsNull(chooser.Target);
        }
        [TestMethod]
        public void NullTargetShouldNotFireEvent()
        {
            var game = new Game();
            var action = new ActionChooseRandomTarget(GameActor.Alignment.Player);
            var actor = new GameActor();

            GameEvents.Instance.TargetSelected += a =>
            {
                Assert.Fail("target should not be fired as chosen if null");
            };
            action.act(game, actor);
        }
    }
}
