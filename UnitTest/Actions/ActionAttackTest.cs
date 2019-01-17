using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest.Actions
{
    [TestClass]
    public class ActionAttackTest
    {
        //KAI: many of these are really integration tests
        [TestMethod]
        public void AttackShouldInflictWeaponDamage()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.ArmPlayer();
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.DoTurn();

            Assert.AreEqual(testGame.mob.Health, testGame.mob.baseHealth - testGame.player.Weapon.damage);
        }
        [TestMethod]
        public void WeaponlessAttackShouldDoNothing()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.DoTurn();

            Assert.AreEqual(testGame.mob.Health, testGame.mob.baseHealth);
        }
        [TestMethod]
        public void UnpreparedAttackEvenWithWeaponShouldDoNothing()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.ArmPlayer();
            testGame.game.DoTurn();

            Assert.AreEqual(testGame.mob.Health, testGame.mob.baseHealth);
        }
    }
}
