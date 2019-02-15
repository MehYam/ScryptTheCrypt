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
        public GameActor playerAlice = new GameActor("alice");
        public GameActor playerBob = new GameActor("bob");
        public GameActor mobCarly = new GameActor("carly");
        public GameActor mobDenise = new GameActor("denise");
        public MockAction playerMockAction = new MockAction();
        public MockAction mobMockAction = new MockAction();
        public TestGameWithActors(int seed = 2112)
        {
            game = new Game(seed);

            playerAlice.AddAction(playerMockAction);
            mobCarly.AddAction(mobMockAction);

            game.AddActor(playerAlice, Game.ActorAlignment.Player);
            game.AddActor(playerBob, Game.ActorAlignment.Player);
            game.AddActor(mobCarly, Game.ActorAlignment.Mob);
            game.AddActor(mobDenise, Game.ActorAlignment.Mob);
        }
        public void ArmAlice(float damage = 20)
        {
            playerAlice.Weapon = new GameWeapon("fist of testing rage", damage);
        }
        public void AddChooseTargetToPlayer()
        {
            playerAlice.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
        }
        public void AddTargetAndAttackToPlayer()
        {
            playerAlice.Target = mobCarly;
            playerAlice.AddAction(new ActionAttack());
        }
        public void KillPlayers()
        {
            playerAlice.TakeDamage(playerAlice.Health);
            playerBob.TakeDamage(playerBob.Health);
        }
        public void KillMobs()
        {
            mobCarly.TakeDamage(mobCarly.Health);
            mobDenise.TakeDamage(mobDenise.Health);
        }
    }
}
