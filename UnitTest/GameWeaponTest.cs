using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameWeaponTest
    {
        [TestMethod]
        public void TestConstruction()
        {
            var weapon = new GameWeapon("snowball", 10);

            Assert.AreEqual(weapon.name, "snowball");
            Assert.AreEqual(weapon.damage, 10);
        }
        [TestMethod]
        public void ConstructorAcceptsZeroDamage()
        {
            var weapon = new GameWeapon("foot", 0);
            Assert.AreEqual(weapon.damage, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsOnNegativeDamage()
        {
            var notOk = new GameWeapon("fist", -1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullName()
        {
            var notOk = new GameWeapon(null, 0);
        }
    }
}
