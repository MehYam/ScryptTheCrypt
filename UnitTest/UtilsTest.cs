using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt.Utils;
namespace UnitTest
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void RNGProducesPredictableNumbersForSeed()
        {
            void tester(int seed, int[] expected)
            {
                var rng = new RNG(seed);
                for (var i = 0; i < expected.Length; ++i)
                {
                    var random = rng.Next();
                    Assert.AreEqual(random, expected[i]);
                }

            }

            tester(2112, new int[] { 185096988, 394131862, 503607836, 197180722, 523558832 });
            tester(0, new int[] { 540338061, 196741933, 988325069, 975536278, 131784020 });
            tester(RNG.MAX - 1, new int[] { 923331827, 175804052, 525401734, 896594342, 84825676 });
        }
    }
}
