using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameActorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void constructor_with_zero_health_should_throwArgumentOutOfRange()
        {
            var actorNotOk = new GameActor(0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void constructor_with_negative_health_should_throwArgumentOutOfRange()
        {
            var actorNotOk = new GameActor(-1);
        }
        [TestMethod]
        public void big_damage_should_zero_health()
        {
            var actor = new GameActor(100);
            actor.takeDamage(1000);
            Assert.AreEqual(actor.health, 0);
        }
        [TestMethod]
        public void big_heal_should_maximize_health()
        {
            var actor = new GameActor(100);
            actor.takeDamage(50);
            actor.heal(1000);
            Assert.AreEqual(actor.health, actor.baseHealth);
        }
    }
}
