using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using kaiGameUtil;
using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class ScryptUtilTest
    {
        [TestMethod]
        public void ILoveIList()
        {
            var list = new List<int> { 2, 3, 4 };
            IList<int> ilist = list;
            var asArray = ScryptUtil.IListToArray(ilist);

            Assert.IsTrue(list.SequenceEqual(asArray));
        }
        [TestMethod]
        public void ChooseRandomLivingShouldChoosePseudoRandomly()
        {
            var test = new TestGameWithActors();
            var jint = new Jint.Engine();
            jint.Execute(ScryptUtil.chooseRandom.body);

            var rng = new RNG(0);
            var players = ScryptUtil.IListToArray(test.game.Players);

            void TestChooseRandom(GameActor actor)
            {
                Assert.AreEqual(actor, jint.Invoke(ScryptUtil.chooseRandom.name, players, rng).ToObject());
            }
            TestChooseRandom(test.playerBob);
            TestChooseRandom(test.playerAlice);
            TestChooseRandom(test.playerBob);
            TestChooseRandom(test.playerBob);
            TestChooseRandom(test.playerAlice);
        }
        [TestMethod]
        public void ChooseRandomLivingShouldNotChooseDead()
        {
            var test = new TestGameWithActors();
            test.playerBob.TakeDamage(test.mobCarly.baseHealth);
            test.playerAlice.TakeDamage(test.mobDenise.baseHealth);

            var jint = new Jint.Engine();
            jint.Execute(ScryptUtil.chooseRandom.body);

            var rng = new RNG(0);
            var players = ScryptUtil.IListToArray(test.game.Players);

            var choice  = jint.Invoke(ScryptUtil.chooseRandom.name, players, rng);

            Assert.IsTrue(choice.IsNull());
        }
        [TestMethod]
        public void AttackTargetShouldDealDamage()
        {
            var test = new TestGameWithActors();
            test.playerAlice.Target = test.mobCarly;
            test.ArmAlice();

            var jint = new Jint.Engine();
            jint.Execute(ScryptUtil.attackTarget.body);
            jint.Invoke(ScryptUtil.attackTarget.name, test.game, test.playerAlice);

            Assert.AreEqual(test.mobCarly.Health, test.mobCarly.baseHealth - test.playerAlice.Weapon.damage);
        }
    }
}
