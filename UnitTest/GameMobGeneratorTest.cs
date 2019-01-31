using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using kaiGameUtil;
using ScryptTheCrypt;
namespace UnitTest
{
    [TestClass]
    public class GameMobGeneratorTest
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void FromStringShouldThrowNullWithNoString()
        {
            var gen = GameMobGenerator.FromString(null);
        }
        [ExpectedException(typeof(FormatException))]
        [TestMethod]
        public void FromStringShouldThrowOnBadlyFormedString()
        {
            var gen = GameMobGenerator.FromString("whatisthis Idon'teven");
        }
        [ExpectedException(typeof(FormatException))]
        [TestMethod]
        public void FromStringShouldThrowOnBadlyFormedStringParts()
        {
            var gen = GameMobGenerator.FromString("mole, notanumber5, teeth, 5");
        }
        [TestMethod]
        public void FromStringShouldCreateMobSpec()
        {
            var gen = GameMobGenerator.FromString("mole, 10, teeth, 5");

            Assert.IsNotNull(gen);
            Assert.AreEqual(1, gen.mobSpec.Count);

            var mob = gen.mobSpec[0];
            Assert.AreEqual(mob.name, "mole");
            Assert.AreEqual(mob.baseHealth, 10);
            Assert.AreEqual(mob.Weapon.name, "teeth");
            Assert.AreEqual(mob.Weapon.damage, 5);
        }
        [TestMethod]
        public void GenerateMobsShouldCreateRandomMobList()
        {
            const int NUM = 10;

            var gen = GameMobGenerator.FromString("mole, 10, teeth, 5\nrat, 11, claw, 6");
            var rng = new RNG(2112);
            var mobs = gen.GenerateMobs(NUM, rng);

            Assert.AreEqual(NUM, mobs.Count);

            Assert.IsTrue(mobs.Exists(mob => mob.name == "mole"));
            Assert.IsTrue(mobs.Exists(mob => mob.name == "rat"));

            var mobs2 = gen.GenerateMobs(NUM, rng);
            Assert.IsTrue(mobs.FindAll(mob => mob.name == "mole").Count != mobs2.FindAll(mob => mob.name == "mole").Count);
        }
    }
}
