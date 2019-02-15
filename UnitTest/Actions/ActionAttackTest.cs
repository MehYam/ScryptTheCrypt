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
            testGame.ArmAlice();
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.mobCarly.Health, testGame.mobCarly.baseHealth - testGame.playerAlice.Weapon.damage);
        }
        [TestMethod]
        public void WeaponlessAttackShouldDoNothing()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.AddTargetAndAttackToPlayer();
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.mobCarly.Health, testGame.mobCarly.baseHealth);
        }
        [TestMethod]
        public void UnpreparedAttackEvenWithWeaponShouldDoNothing()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.ArmAlice();
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.mobCarly.Health, testGame.mobCarly.baseHealth);
        }
    }
}
