using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest
{
    //KAI: so here's where we look into Moq
    class MockAction : IActorAction
    {
        public Game gCalledWith;
        public GameActor aCalledWith;
        public int timesCalled = 0;
        public void act(Game g, GameActor actor)
        {
            gCalledWith = g;
            aCalledWith = actor;
            ++timesCalled;
        }
    }
    class TestGameWithActors
    {
        public Game game;
        public GameActor player = new GameActor("alice");
        public GameActor player2 = new GameActor("bob");
        public GameActor mob = new GameActor("carly");
        public GameActor mob2 = new GameActor("denise");
        public MockAction playerMockAction = new MockAction();
        public MockAction mobMockAction = new MockAction();
        public TestGameWithActors(int seed = 2112)
        {
            game = new Game(seed);

            player.AddAction(playerMockAction);
            mob.AddAction(mobMockAction);

            game.players.Add(player);
            game.players.Add(player2);
            game.mobs.Add(mob);
            game.mobs.Add(mob2);
        }
        public void ArmPlayer(float damage = 20)
        {
            player.Weapon = new GameWeapon("fist of testing rage", damage);
        }
        public void PreparePlayerToAttack()
        {
            player.SetAttribute(GameActor.Attribute.Target, mob);
            player.AddAction(new ActionAttack());
        }
    }
}
