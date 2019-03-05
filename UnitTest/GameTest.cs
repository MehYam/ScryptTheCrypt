using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using KaiGameUtil;
using ScryptTheCrypt;

namespace UnitTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void TurnWithoutPlayersShouldBeOk()
        {
            var game = new Game(1000);
            game.PlayRound();
        }
        TestGameWithActors testData = null;
        [TestInitialize]
        public void TestInitialize()
        {
            testData = new TestGameWithActors();
        }
        [TestCleanup]
        public void TestCleanup()
        {
            testData = null;
            GameEvents.ReleaseAllListeners();
        }
        [TestMethod]
        public void NumRoundsShouldStartZero()
        {
            Assert.AreEqual(0, testData.game.NumRounds);
        }
        [TestMethod]
        public void PlayRoundShouldIncrementNumRounds()
        {
            testData.game.PlayRound();

            Assert.AreEqual(1, testData.game.NumRounds);
        }
        [TestMethod]
        public void EnumerateRoundShouldIncrementNumRounds()
        {
            var actions = testData.game.EnumerateRound();
            Assert.AreEqual(0, testData.game.NumRounds);

            while (actions.MoveNext())
            {
                Assert.AreEqual(1, testData.game.NumRounds);
            }

            Assert.AreEqual(1, testData.game.NumRounds);
        }
        [TestMethod]
        public void RoundStartShouldFireAfterNumRoundsSet()
        {
            GameEvents.Instance.RoundStart += g =>
            {
                Assert.AreEqual(1, testData.game.NumRounds);
            };
            testData.game.PlayRound();
        }
        private void TestAddActor(GameActor.Alignment align)
        {
            var game = new Game();
            var actor1 = new GameActor(align, "one");
            var actor2 = new GameActor(align, "two");
            game.AddActor(actor1);
            game.AddActor(actor2);

            var coll = align == GameActor.Alignment.Player ? game.Players : game.Mobs;
            Assert.AreEqual(coll[0], actor1);
            Assert.AreEqual(coll[1], actor2);
        }
        [TestMethod]
        public void AddActorShouldAddPlayersInOrderAndInCorrectGroup()
        {
            TestAddActor(GameActor.Alignment.Player);
        }
        [TestMethod]
        public void AddActorShouldAddMobsInOrderAndInCorrectGroup()
        {
            TestAddActor(GameActor.Alignment.Mob);
        }
        private void TestAddActorEvent(GameActor.Alignment align)
        {
            var game = new Game();
            var actor = new GameActor();
            int called = 0;
            GameEvents.Instance.ActorAdded += (g, a) =>
            {
                ++called;
                Assert.AreEqual(game, g);
                Assert.AreEqual(a, actor);
            };
            game.AddActor(actor);
            Assert.AreEqual(1, called);
        }
        [TestMethod]
        public void AddActorShouldFireEvent()
        {
            TestAddActorEvent(GameActor.Alignment.Player);
        }
        [TestMethod]
        public void AddMobShouldFireEvent()
        {
            TestAddActorEvent(GameActor.Alignment.Mob);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PlayerListShouldBeReadOnly()
        {
            testData.game.Players[0] = null;
        }
        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public void MobListShouldBeReadOnly()
        {
            testData.game.Mobs[0] = null;
        }
        [TestMethod]
        public void ClearActorsShouldEmptyActorList()
        {
            Assert.IsTrue(testData.game.Players.Count > 0);
            Assert.IsTrue(testData.game.Mobs.Count > 0);

            testData.game.ClearActors(GameActor.Alignment.Mob);
            Assert.IsTrue(testData.game.Players.Count > 0);
            Assert.AreEqual(0, testData.game.Mobs.Count);

            testData.game.ClearActors(GameActor.Alignment.Player);
            Assert.AreEqual(0, testData.game.Players.Count);
        }
        [TestMethod]
        public void ClearActorsShouldFireEvent()
        {
            int players = testData.game.Players.Count;
            int mobs = testData.game.Mobs.Count;

            Assert.IsTrue(players > 0);
            Assert.IsTrue(mobs > 0);

            GameEvents.Instance.ActorRemoved += (g, a) =>
            {
                if (a.align == GameActor.Alignment.Player)
                {
                    --players;
                    Assert.IsFalse(testData.game.Players.Contains(a));
                }
                if (a.align == GameActor.Alignment.Mob)
                {
                    --mobs;
                    Assert.IsFalse(testData.game.Mobs.Contains(a));
                }
            };

            testData.game.ClearActors(GameActor.Alignment.Player);
            Assert.AreEqual(0, players);

            testData.game.ClearActors(GameActor.Alignment.Mob);
            Assert.AreEqual(0, mobs);
        }
        [TestMethod]
        public void DeadMobsShouldReturnVictory()
        {
            testData.KillMobs();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.PlayersWin);
        }
        [TestMethod]
        public void DeadPlayersShouldReturnLoss()
        {
            testData.KillPlayers();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.MobsWin);
        }
        [TestMethod]
        public void AllDeadShouldReturnDraw()
        {
            testData.KillPlayers();
            testData.KillMobs();

            Assert.AreEqual(testData.game.GameProgress, Game.Progress.Draw);
        }
        [TestMethod]
        public void SurvivingPlayerAndMobShouldReturnGameInProgress()
        {
            Assert.AreEqual(testData.game.GameProgress, Game.Progress.InProgress);
        }
        [TestMethod]
        public void DeadActorsShouldGetNoTurn()
        {
            testData.playerAlice.TakeDamage(testData.playerAlice.Health);
            testData.game.PlayRound();

            Assert.AreEqual(testData.playerMockAction.timesCalled, 0);
            Assert.AreEqual(testData.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectGame()
        {
            testData.game.PlayRound();

            Assert.AreEqual(testData.playerMockAction.gCalledWith, testData.game);
            Assert.AreEqual(testData.mobMockAction.gCalledWith, testData.game);
        }
        [TestMethod]
        public void DoRoundShouldCallActionWithCorrectActor()
        {
            testData.game.PlayRound();
            Assert.AreEqual(testData.playerMockAction.aCalledWith, testData.playerAlice);
            Assert.AreEqual(testData.mobMockAction.aCalledWith, testData.mobCarly);
        }
        [TestMethod]
        public void DoRoundShouldCallActionOnce()
        {
            testData.game.PlayRound();
            Assert.AreEqual(testData.playerMockAction.timesCalled, 1);
            Assert.AreEqual(testData.mobMockAction.timesCalled, 1);
        }
        [TestMethod]
        public void DoRoundShouldFireTurnEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.RoundStart += g => { startFired = true; };
            GameEvents.Instance.RoundEnd += g => { endFired = true; };
            testData.game.PlayRound();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoRoundShouldFireActorActionsEvents()
        {
            bool fired = false;
            GameActor currentActor = null;
            GameEvents.Instance.ActorActionsStart += (g, a) =>
            {
                fired = true;
                Assert.AreEqual(g, testData.game);
                Assert.IsNull(currentActor);
                currentActor = a;
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) =>
            {
                Assert.AreEqual(g, testData.game);
                Assert.AreEqual(a, currentActor);
                currentActor = null;
            };

            testData.game.PlayRound();

            Assert.IsTrue(fired);
            Assert.IsNull(currentActor);
        }
        [TestMethod]
        public void GetTargetsShouldReturnTargetedActors()
        {
            GameActor actor1 = new GameActor();
            GameActor actor2 = new GameActor();
            GameActor actor3 = new GameActor();

            actor1.Targeted = true;
            actor2.Targeted = true;

            var game = new Game();
            game.AddActor(actor1);
            game.AddActor(actor2);
            game.AddActor(actor3);

            var targets = game.GetTargets();

            Assert.AreEqual(2, targets.Count);
            Assert.IsTrue(targets.Contains(actor1));
            Assert.IsTrue(targets.Contains(actor2));
        }
        [TestMethod]
        public void GetPositionsShouldReturnActorPositions()
        {
            GameActor actor1 = new GameActor();
            GameActor actor2 = new GameActor();

            actor1.pos = new Point<int>(-3, 3);
            actor2.pos = new Point<int>(10, 10);

            var game = new Game();
            game.AddActor(actor1);
            game.AddActor(actor2);

            var field = game.GetPositions();

            Assert.AreEqual(field.size.x, Math.Abs(actor1.pos.x - actor2.pos.x) + 1);
            Assert.AreEqual(field.size.y, Math.Abs(actor1.pos.y - actor2.pos.y) + 1);

            int total = 0;
            field.ForEach((x, y, actor) =>
            {
                if (actor != null)
                {
                    ++total;
                }
                else if (actor == actor1)
                {
                    Assert.AreEqual(x, actor1.pos.x);
                    Assert.AreEqual(y, actor1.pos.x);
                }
                else if (actor == actor2)
                {
                    Assert.AreEqual(x, actor2.pos.x);
                    Assert.AreEqual(y, actor2.pos.x);
                }
            });
            Assert.AreEqual(2, total);
        }
        // KAI: we're mixing unit and integration tests, look into how to manage that
        [TestMethod]
        public void DoRoundShouldFireAttackEvents()
        {
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.AttackStart += (a, b) =>
            {
                Assert.AreEqual(a, testData.playerAlice);
                Assert.AreEqual(b, testData.mobCarly);
                startFired = true;
            };
            GameEvents.Instance.AttackEnd += (a, b) =>
            {
                Assert.AreEqual(a, testData.playerAlice);
                Assert.AreEqual(b, testData.mobCarly);
                endFired = true;
            };
            testData.ArmAlice();
            testData.AggroAliceAndTargetCarly();
            testData.game.PlayRound();

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void DoRoundShouldFireDeathEvents()
        {
            bool fired = false;
            GameEvents.Instance.Death += a =>
            {
                fired = true;
                Assert.AreEqual(a, testData.mobCarly);
            };
            testData.ArmAlice(testData.mobCarly.baseHealth);
            testData.AggroAliceAndTargetCarly();
            testData.game.PlayRound();
            Assert.IsTrue(fired);
            Assert.IsFalse(testData.mobCarly.Alive);
        }
        [TestMethod]
        public void DoRoundShouldFireTargetEvents()
        {
            bool fired = false;
            GameEvents.Instance.ActorTargetedChange += a =>
            {
                fired = true;
            };
            testData.AddChooseTargetToPlayer();
            testData.game.PlayRound();
            Assert.IsTrue(fired);
        }
        [TestMethod]
        public void EnumerateTurnShouldRunAllActions()
        {
            var game = new Game();
            var player = new GameActor(GameActor.Alignment.Player);
            var mob = new GameActor(GameActor.Alignment.Mob);

            var mock1 = new MockAction();
            var mock2 = new MockAction();
            player.AddAction(mock1);
            mob.AddAction(mock2);
            game.AddActor(player);
            game.AddActor(mob);

            var turns = game.EnumerateRound();
            while(turns.MoveNext());

            Assert.AreEqual(1, mock1.timesCalled);
            Assert.AreEqual(1, mock2.timesCalled);
            Assert.IsTrue(mock1.orderCalledIn < mock2.orderCalledIn);
        }
        [TestMethod]
        public void EnumerateTurnShouldFireTurnEvents()
        {
            var game = new Game();
            bool startFired = false;
            bool endFired = false;
            GameEvents.Instance.RoundStart += g =>
            {
                Assert.IsFalse(endFired);
                startFired = true;
            };
            GameEvents.Instance.RoundEnd += g =>
            {
                Assert.IsTrue(startFired);
                endFired = true;
            };
            var turns = game.EnumerateRound();
            while(turns.MoveNext());

            Assert.IsTrue(startFired);
            Assert.IsTrue(endFired);
        }
        [TestMethod]
        public void JintCanAccessGame()
        {
            var testGame = new Game();

            var script = new Jint.Engine();
            script.Execute(@"
                function accessGame(game) {
                    game.PlayRound()
                }
            ");
            script.Invoke("accessGame", testGame);
            Assert.AreEqual(1, testGame.NumRounds);
        }
        static private Game CreateTestGameWithScrypt()
        {
            var testGame = new Game();
            var testActor = new GameActor();

            testActor.SetScrypt(@"
            function actorActions(game, actor) {
                actor.TakeDamage(5);
            }
            ");

            testGame.AddActor(testActor);
            return testGame;
        }
        [TestMethod]
        public void EnumerateRound_ScryptShouldRunActorScrypts()
        {
            var testGame = CreateTestGameWithScrypt();
            var rounds = testGame.EnumerateRound_Scrypt();
            while(rounds.MoveNext());

            var testActor = testGame.Players[0];
            Assert.AreEqual(testActor.baseHealth - 5, testActor.Health);
        }
        [TestMethod]
        public void PlayRound_ScryptShouldRunActorScrypts()
        {
            var testGame = CreateTestGameWithScrypt();
            testGame.PlayRound_Scrypt();

            var testActor = testGame.Players[0];
            Assert.AreEqual(testActor.baseHealth - 5, testActor.Health);
        }
    }
}
