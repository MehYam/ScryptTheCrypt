using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;
namespace UnitTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void GameShouldProduceWaveOfEnemies()
        {
            var game = new Game(2112);
            game.Start();

            Assert.IsNotNull(game.CurrentBattle);
        }
        [TestMethod]
        public void GameShouldProduceContent()
        {
            Assert.Fail();
        }
    }
}
