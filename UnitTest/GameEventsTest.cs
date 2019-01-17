using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameEventsTest
    {
        [TestMethod]
        public void EventShouldFireOnlyWhenSubscribed()
        {
            int timesFired = 0;
            var game = new Game();
            void handler(Game g)
            {
                ++timesFired;
                Assert.AreEqual(game, g);
            }

            GameEvents.Instance.TurnStart += handler;
            GameEvents.Instance.TurnStart_Fire(game);

            Assert.AreEqual(timesFired, 1);

            GameEvents.Instance.TurnStart -= handler;
            GameEvents.Instance.TurnStart_Fire(game);

            Assert.AreEqual(timesFired, 1);
        }
        [TestMethod]
        public void ReleaseAllListenersShouldUnsubscribe()
        {
            int timesFired = 0;
            void MyTurnStartHandler(Game g)
            {
                ++timesFired;
            }

            GameEvents.Instance.TurnStart += MyTurnStartHandler;
            GameEvents.Instance.TurnStart_Fire(null);
            GameEvents.ReleaseAllListeners();
            GameEvents.Instance.TurnStart_Fire(null);

            Assert.AreEqual(timesFired, 1);
        }
        [TestMethod]
        public void EndOfGameShouldFireEvent()
        {

        }
    }
}
