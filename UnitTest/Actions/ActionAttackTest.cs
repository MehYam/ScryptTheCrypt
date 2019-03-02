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
        public void AttackShouldInflictWeaponDamageOnAllTargets()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.ArmAlice();
            testGame.AggroAliceAndTargetCarly();
            testGame.mobDenise.Targeted = true;
            testGame.game.PlayRound();

            Assert.AreEqual(testGame.mobCarly.baseHealth - testGame.playerAlice.Weapon.damage, testGame.mobCarly.Health);
            Assert.AreEqual(testGame.mobDenise.baseHealth - testGame.playerAlice.Weapon.damage, testGame.mobDenise.Health);
        }
        [TestMethod]
        public void WeaponlessAttackShouldDoNothing()
        {
            var testGame = new TestGameWithActors(2112);
            testGame.AggroAliceAndTargetCarly();
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
