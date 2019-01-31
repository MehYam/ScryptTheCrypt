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
        public int orderCalledIn = 0;

        static private int calls = 0;
        public void act(Game g, GameActor actor)
        {
            gCalledWith = g;
            aCalledWith = actor;
            ++timesCalled;
            orderCalledIn = ++MockAction.calls;
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

            game.AddActor(player, Game.ActorAlignment.Player);
            game.AddActor(player2, Game.ActorAlignment.Player);
            game.AddActor(mob, Game.ActorAlignment.Mob);
            game.AddActor(mob2, Game.ActorAlignment.Mob);
        }
        public void ArmPlayer(float damage = 20)
        {
            player.Weapon = new GameWeapon("fist of testing rage", damage);
        }
        public void AddChooseTargetToPlayer()
        {
            player.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
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
