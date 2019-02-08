using Microsoft.VisualStudio.TestTools.UnitTesting;

using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameEventsTest
    {
        [TestCleanup]
        public void TestCleanup()
        {
            GameEvents.ReleaseAllListeners();
        }
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

            GameEvents.Instance.RoundStart += handler;
            GameEvents.Instance.RoundStart_Fire(game);

            Assert.AreEqual(timesFired, 1);

            GameEvents.Instance.RoundStart -= handler;
            GameEvents.Instance.RoundStart_Fire(game);

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

            GameEvents.Instance.RoundStart += MyTurnStartHandler;
            GameEvents.Instance.RoundStart_Fire(null);
            GameEvents.ReleaseAllListeners();
            GameEvents.Instance.RoundStart_Fire(null);

            Assert.AreEqual(timesFired, 1);
        }
        [TestMethod]
        public void MoonsharpCanAccessGame()
        {
            var script = new MoonSharp.Interpreter.Script();
            script.DoString(@"
                function fireGameEvent(events, game)
                    events.RoundStart_Fire(game)
                end
            ");

            var game = new Game();
            bool fired = false;
            GameEvents.Instance.RoundStart += g =>
            {
                fired = true;
                Assert.AreEqual(game, g);
            };
            script.Call(script.Globals["fireGameEvent"], GameEvents.Instance, game);
            Assert.IsTrue(fired);
        }
    }
}
