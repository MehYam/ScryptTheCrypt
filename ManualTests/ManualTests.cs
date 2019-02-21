using System;
using System.Reflection;
using System.Text;
using kaiGameUtil;
using ScryptTheCrypt;
using ScryptTheCrypt.Actions;
namespace ManualTests
{
    class ManualTests
    {
        static bool verbose = false;
        static void Main(string[] args)
        {
            var dlls = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var dll in dlls)
            {
                Console.WriteLine($"loaded: {dll.Name} version {dll.Version}");
            }

            verbose = Array.Exists(args, arg => arg.ToLower() == "verbose");

            //RunSampleGame1();
            //RunSampleGame2();
            RunSampleGame_Scrypt();
        }
        static void RNG()
        {
            var rng = new RNG(2112);

            for (var i = 0; i < 100; ++i)
            {
                Console.WriteLine(rng.NextDouble());
                Console.WriteLine(rng.Next(3, 5));
            }
        }
        static void RunSampleGame1()
        {
            Console.WriteLine("input a seed (2112): ");
            var strSeed = Console.ReadLine();
            if (!int.TryParse(strSeed, out int seed))
            {
                seed = 2112;
            }
            RunGame(CreateSampleGame1(seed), null, GameMobGenerator.ActionType.IAction);
        }
        static void RunSampleGame2()
        {
            Console.WriteLine("input a seed (2112): ");
            var strSeed = Console.ReadLine();
            if (!int.TryParse(strSeed, out int seed))
            {
                seed = 2112;
            }

            var rng = new RNG(seed);
            var mobGen = new GameMobGenerator(
                rng,
                () => new GameActor(GameActor.Alignment.Mob, "rat", 10, new GameWeapon("teeth", 4)),
                () => new GameActor(GameActor.Alignment.Mob, "mole", 8, new GameWeapon("claw", 6)),
                () => new GameActor(GameActor.Alignment.Mob, "lynx", 15, new GameWeapon("pounce", 10))
            );
            RunGame(CreateSampleGame2(rng), mobGen, GameMobGenerator.ActionType.IAction);
        }
        static void RunSampleGame_Scrypt()
        {
            Console.WriteLine("input a seed (2112): ");
            var strSeed = Console.ReadLine();
            if (!int.TryParse(strSeed, out int seed))
            {
                seed = 2112;
            }

            var rng = new RNG(seed);
            var mobGen = new GameMobGenerator(
                rng,
                () => new GameActor(GameActor.Alignment.Mob, "rat", 10, new GameWeapon("teeth", 4)),
                () => new GameActor(GameActor.Alignment.Mob, "mole", 8, new GameWeapon("claw", 6)),
                () => new GameActor(GameActor.Alignment.Mob, "lynx", 15, new GameWeapon("pounce", 10))
            );
            RunGame(CreateSampleGame2(rng), mobGen, GameMobGenerator.ActionType.Scrypt);
        }
        class GameMobGenerator
        {
            private readonly Func<GameActor>[] generators;
            private readonly RNG rng;
            public GameMobGenerator(RNG rng, params Func<GameActor>[] generators)
            {
                this.rng = rng;
                this.generators = generators;
            }
            public enum ActionType { None, IAction, Scrypt };
            public GameActor Gen(ActionType t)
            {
                var mob = generators[rng.NextIndex(generators)]();
                switch(t)
                {
                    case ActionType.IAction:
                        mob.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Player));
                        mob.AddAction(new ActionAttack());
                        break;
                    case ActionType.Scrypt:
                        mob.SetScrypt(ScryptUtil.defaultAttack);
                        break;
                }
                return mob;
            }
        }
        static Game CreateSampleGame1(int seed)
        {
            var game = new Game(seed);
            var player = new GameActor(GameActor.Alignment.Player, "alice");
            var player2 = new GameActor(GameActor.Alignment.Player, "bob");
            var mob = new GameActor(GameActor.Alignment.Mob, "carly");
            var mob2 = new GameActor(GameActor.Alignment.Mob, "denise");

            // set targeting and affinities
            player.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Mob));
            player2.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Mob));
            mob.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Player));
            mob2.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Player));

            player.AddAction(new ActionAttack());
            player2.AddAction(new ActionAttack());
            mob.AddAction(new ActionAttack());
            mob2.AddAction(new ActionAttack());

            player.Weapon = new GameWeapon("alice's axe", 22);
            player2.Weapon = new GameWeapon("bob's burger", 12);
            mob.Weapon = new GameWeapon("carly's cutlass", 33);
            mob2.Weapon = new GameWeapon("denise's dog", 5);

            game.AddActor(player);
            game.AddActor(player2);
            game.AddActor(mob);
            game.AddActor(mob2);
            return game;
        }
        static Game CreateSampleGame2(RNG rng)
        {
            var game = new Game(rng);
            GameActor CreatePlayer(string name, string weaponName, int weaponDmg)
            {
                var player = new GameActor(GameActor.Alignment.Player, name, 10) {
                    Weapon = new GameWeapon(weaponName, weaponDmg)
                };
                player.AddAction(new ActionChooseRandomTarget(GameActor.Alignment.Mob));
                player.AddAction(new ActionAttack());
                player.SetScrypt(ScryptUtil.defaultAttack);
                return player;
            }
            game.AddActor(CreatePlayer("Andrew", "ahlspiess", 4));
            game.AddActor(CreatePlayer("Beatrice", "bow", 5));
            game.AddActor(CreatePlayer("Candy", "cutlass", 6));
            game.AddActor(CreatePlayer("Dierdre", "dagger", 3));
            return game;
        }
        static void RunGame(Game game, GameMobGenerator mobGen, GameMobGenerator.ActionType actionType)
        {
            GameEvents.Instance.ActorAdded += (g, a) =>
            {
                Console.WriteLine($"ActorAdded: {a}");
            };
            GameEvents.Instance.RoundStart += g =>
            {
                Console.WriteLine("RoundStart");
                Console.ReadKey();
            };
            GameEvents.Instance.RoundEnd += g =>
            {
                Console.WriteLine("RoundEnd");
                Console.ReadKey();
            };
            GameEvents.Instance.AttackStart += (a, b) =>
            {
                Console.WriteLine($"{a.uniqueName} attacking {b.uniqueName} with {a.Weapon}");
                Console.ReadKey();
            };
            GameEvents.Instance.ActorHealthChange += (a, o, n) =>
            {
                Console.WriteLine($"{a.uniqueName} health {o} => {n}");
                Console.ReadKey();
            };
            GameEvents.Instance.Death += a =>
            {
                Console.WriteLine($"=-=-=RIP=-=-= {a.uniqueName}");
                Console.ReadKey();
            };
            if (verbose)
            {
                GameEvents.Instance.ActorActionsStart += (g, a) =>
                {
                    Console.WriteLine($"{a.uniqueName} starting turn");
                    Console.ReadKey();
                };
                GameEvents.Instance.ActorActionsEnd += (g, a) =>
                {
                    Console.WriteLine($"{a.uniqueName} ending turn");
                    Console.ReadKey();
                };
                GameEvents.Instance.AttackEnd += (a, b) =>
                {
                    Console.WriteLine($"attack finished, target {b.uniqueName} health {b.Health}/{b.baseHealth}");
                    Console.ReadKey();
                };
            }

            while (game.GameProgress != Game.Progress.MobsWin)
            {
                if (mobGen != null)
                {
                    Console.WriteLine("Generating mobs");
                    game.ClearActors(GameActor.Alignment.Mob);
                    for (int i = 0; i < 4; ++i)
                    {
                        game.AddActor(mobGen.Gen(actionType));
                    }
                }
                while (game.GameProgress == Game.Progress.InProgress)
                {
                    Console.WriteLine(game.ToString());
                    Console.WriteLine("Any key to continue...");
                    Console.ReadKey();

                    switch(actionType)
                    {
                        case GameMobGenerator.ActionType.IAction:
                            game.PlayRound();
                            break;
                        case GameMobGenerator.ActionType.Scrypt:
                            game.PlayRound_Scrypt();
                            break;
                    }

                    Console.WriteLine($"Round ended with result {game.GameProgress}");
                    Console.ReadKey();
                }
            }
            Console.WriteLine($"Game ended with result {game.GameProgress}");
            GameEvents.ReleaseAllListeners();
        }
    }
}
