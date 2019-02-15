﻿using System;
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

            testGame.playerAlice.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Player));
            testGame.game.PlayRound();

            Assert.IsNotNull(testGame.playerAlice.Target);
            Assert.IsTrue(testGame.game.Players.Contains(testGame.playerAlice.Target));
        }
        [TestMethod]
        public void PseudoRandomMobTargetShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.playerAlice.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
            testGame.game.PlayRound();

            Assert.IsNotNull(testGame.playerAlice.Target);
            Assert.IsTrue(testGame.game.Mobs.Contains(testGame.playerAlice.Target));
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

            game.AddActor(deadHorse, Game.ActorAlignment.Mob);

            var action = new ActionChooseRandomTarget(Game.ActorAlignment.Mob);
            var chooser = new GameActor();

            game.AddActor(chooser, Game.ActorAlignment.Player);
            action.act(game, chooser);

            Assert.IsNull(chooser.Target);
        }
        [TestMethod]
        public void NullTargetShouldNotFireEvent()
        {
            var game = new Game();
            var action = new ActionChooseRandomTarget(Game.ActorAlignment.Player);
            var actor = new GameActor();

            GameEvents.Instance.TargetSelected += a =>
            {
                Assert.Fail("target should not be fired as chosen if null");
            };
            action.act(game, actor);
        }
    }
}
