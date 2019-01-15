using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameWeaponTest
    {
        [TestMethod]
        public void test_construction()
        {
            var weapon = new GameWeapon("snowball", 10);

            Assert.AreEqual(weapon.name, "snowball");
            Assert.AreEqual(weapon.damage, 10);
        }
        [TestMethod]
        public void constructor_with_zero_damage_should_be_fine()
        {
            var weapon = new GameWeapon("foot", 0);
            Assert.AreEqual(weapon.damage, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void constructor_with_negative_damage_should_throwArgumentOutOfRange()
        {
            var notOk = new GameWeapon("fist", -1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void constructor_with_no_name_should_throw()
        {
            var notOk = new GameWeapon(null, 0);
        }
    }
}
