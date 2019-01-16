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
        public void FriendlyTargetFromGameShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);

            testGame.player.AddAction(new ActionChooseRandomTarget(true));
            testGame.game.DoTurn();

            Assert.Fail();
        }
        [TestMethod]
        public void HostileTargetFromGameShouldBeChosen()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.player.AddAction(new ActionChooseRandomTarget(false));
            testGame.game.DoTurn();

            Assert.Fail();
        }
    }
}
