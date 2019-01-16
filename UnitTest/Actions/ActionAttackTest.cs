using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest.Actions
{
    [TestClass]
    public class ActionAttackTest
    {
        [TestMethod]
        public void TestAttack()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.player.SetAttribute(GameActor.Attribute.Target, testGame.mob);
            testGame.player.AddAction(new ActionAttack());
            testGame.player.Weapon = new GameWeapon("fist of testing rage", 20);

            testGame.game.DoTurn();

            Assert.AreEqual(testGame.mob.Health, testGame.mob.baseHealth - testGame.player.Weapon.damage);
        }
    }
}
