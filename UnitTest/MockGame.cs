using ScryptTheCrypt;
using ScryptTheCrypt.Actions;

namespace UnitTest
{
    //KAI: so here's where we look into Moq
    class MockAction : IActorAction
    {
        public GameBattle gCalledWith;
        public GameActor aCalledWith;
        public int timesCalled = 0;
        public int orderCalledIn = 0;

        static private int calls = 0;
        public void act(GameBattle g, GameActor actor)
        {
            gCalledWith = g;
            aCalledWith = actor;
            ++timesCalled;
            orderCalledIn = ++MockAction.calls;
        }
    }
    class TestBattleWithActors
    {
        public GameBattle game;
        public GameActor player = new GameActor("alice");
        public GameActor player2 = new GameActor("bob");
        public GameActor mob = new GameActor("carly");
        public GameActor mob2 = new GameActor("denise");
        public MockAction playerMockAction = new MockAction();
        public MockAction mobMockAction = new MockAction();
        public TestBattleWithActors(int seed = 2112)
        {
            game = new GameBattle(seed);

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
        public void AddChooseTargetToPlayer()
        {
            player.AddAction(new ActionChooseRandomTarget(GameBattle.ActorAlignment.Mob));
        }
        public void AddTargetAndAttackToPlayer()
        {
            player.SetAttribute(GameActor.Attribute.Target, mob);
            player.AddAction(new ActionAttack());
        }
        public void KillPlayers()
        {
            player.TakeDamage(player.Health);
            player2.TakeDamage(player2.Health);
        }
        public void KillMobs()
        {
            mob.TakeDamage(mob.Health);
            mob2.TakeDamage(mob2.Health);
        }
    }
}
