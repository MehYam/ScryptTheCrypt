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

            void TestChooseRandom(GameActor actor)
            {
                Assert.AreEqual(actor, jint.Invoke(ScryptUtil.chooseRandom.name, test.game.Players, rng).ToObject());
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
            var choice  = jint.Invoke(ScryptUtil.chooseRandom.name, test.game.Players, rng);

            Assert.IsTrue(choice.IsNull());
        }
        [TestMethod]
        public void AttackTargetShouldDealDamage()
        {
            var test = new TestGameWithActors();
            test.ArmAlice();
            test.mobCarly.Targeted = true;

            var jint = new Jint.Engine();
            jint.Execute(ScryptUtil.attackTargets.body);
            jint.Invoke(ScryptUtil.attackTargets.name, test.game, test.playerAlice);

            Assert.AreEqual(test.mobCarly.Health, test.mobCarly.baseHealth - test.playerAlice.Weapon.damage);
        }
        [TestMethod]
        public void AttackTargetShouldClearTargetAfterAttack()
        {
            var test = new TestGameWithActors();
            test.ArmAlice();
            test.AggroAliceAndTargetCarly();

            var jint = new Jint.Engine();
            jint.Execute(ScryptUtil.attackTargets.body);
            jint.Invoke(ScryptUtil.attackTargets.name, test.game, test.playerAlice);

            Assert.IsFalse(test.mobCarly.Targeted);
        }
        [TestMethod]
        public void DefaultAttackScryptShouldAttack()
        {
            var test = new TestGameWithActors();
            test.ArmAlice();

            test.playerAlice.SetScrypt(ScryptUtil.defaultAttack);
            test.playerAlice.SetScryptLogger(new Action<string>(s => { Console.WriteLine($"scrypt log: {s}"); } ));
            test.game.PlayRound_Scrypt();

            Assert.AreNotEqual(0, test.game.Mobs.Where(a => a.Health < a.baseHealth).Count());
        }
    }
}
