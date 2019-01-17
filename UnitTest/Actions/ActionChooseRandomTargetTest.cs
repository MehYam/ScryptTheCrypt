using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest.Actions
{
    [TestClass]
    public class ActionChooseRandomTargetTest
    {
        [TestMethod]
        public void PseudoRandomPlayerTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.player.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Player));
            testGame.game.DoTurn();

            var target = testGame.player.GetAttribute(GameActor.Attribute.Target);

            Assert.IsInstanceOfType(target, typeof(GameActor));
            Assert.IsTrue(testGame.game.players.Contains(target as GameActor));
        }
        [TestMethod]
        public void PseudoRandomMobTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.player.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
            testGame.game.DoTurn();

            var target = testGame.player.GetAttribute(GameActor.Attribute.Target);

            Assert.IsInstanceOfType(target, typeof(GameActor));
            Assert.IsTrue(testGame.game.mobs.Contains(target as GameActor));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallingActWithNoGameThrows()
        {
            var action = new ActionChooseRandomTarget(Game.ActorAlignment.Player);
            action.act(null, new GameActor());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallingActWithNoActorThrows()
        {
            var action = new ActionChooseRandomTarget(Game.ActorAlignment.Player);
            action.act(new Game(), null);
        }
        [TestMethod]
        public void DeadActorsShouldNotBeTargeted()
        {
            var game = new Game();
            var deadHorse = new GameActor();
            deadHorse.TakeDamage(deadHorse.Health);

            game.mobs.Add(deadHorse);

            var action = new ActionChooseRandomTarget(Game.ActorAlignment.Mob);
            var chooser = new GameActor();

            game.players.Add(chooser);
            action.act(game, chooser);

            Assert.IsNull(chooser.GetAttribute(GameActor.Attribute.Target));
        }
    }
}
