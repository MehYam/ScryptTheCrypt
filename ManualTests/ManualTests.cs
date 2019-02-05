using System;
using System.Collections.Generic;

using kaiGameUtil;
using ScryptTheCrypt;
using ScryptTheCrypt.Actions;
namespace ManualTests
{
    class ManualTests
    {
        static void Main(string[] args)
        {
            //RunSampleGame1();
            RunSampleGame2();
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
            RunGame(CreateSampleGame1(seed), null, true);
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
            public GameActor Gen(bool addDefaultAttack)
            {
                var retval = generators[rng.NextIndex(generators)]();
                if (addDefaultAttack)
                {
                    retval.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Player));
                    retval.AddAction(new ActionAttack());
                }
                return retval;
            }
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
                () => new GameActor("rat", 10, new GameWeapon("teeth", 4)),
                () => new GameActor("mole", 8, new GameWeapon("claw", 6)),
                () => new GameActor("lynx", 15, new GameWeapon("pounce", 10))
            );
            RunGame(CreateSampleGame2(rng), mobGen, false);
        }
        static Game CreateSampleGame1(int seed)
        {
            var game = new Game(seed);
            var player = new GameActor("alice");
            var player2 = new GameActor("bob");
            var mob = new GameActor("carly");
            var mob2 = new GameActor("denise");

            // set targeting and affinities
            player.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
            player2.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
            mob.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Player));
            mob2.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Player));

            player.AddAction(new ActionAttack());
            player2.AddAction(new ActionAttack());
            mob.AddAction(new ActionAttack());
            mob2.AddAction(new ActionAttack());

            player.Weapon = new GameWeapon("alice's axe", 22);
            player2.Weapon = new GameWeapon("bob's burger", 12);
            mob.Weapon = new GameWeapon("carly's cutlass", 33);
            mob2.Weapon = new GameWeapon("denise's dog", 5);

            game.AddActor(player, Game.ActorAlignment.Player);
            game.AddActor(player2, Game.ActorAlignment.Player);
            game.AddActor(mob, Game.ActorAlignment.Mob);
            game.AddActor(mob2, Game.ActorAlignment.Mob);
            return game;
        }
        static Game CreateSampleGame2(RNG rng)
        {
            var game = new Game(rng);
            GameActor CreatePlayer(string name, string weaponName, int weaponDmg)
            {
                var retval = new GameActor(name) {
                    Weapon = new GameWeapon(weaponName, weaponDmg)
                };
                retval.AddAction(new ActionChooseRandomTarget(Game.ActorAlignment.Mob));
                retval.AddAction(new ActionAttack());
                return retval;
            }
            game.AddActor(CreatePlayer("Andrew", "ahlspiess", 4), Game.ActorAlignment.Player);
            game.AddActor(CreatePlayer("Beatrice", "bow", 5), Game.ActorAlignment.Player);
            game.AddActor(CreatePlayer("Candy", "cutlass", 6), Game.ActorAlignment.Player);
            game.AddActor(CreatePlayer("Dierdre", "dagger", 3), Game.ActorAlignment.Player);
            return game;
        }
        static void RunGame(Game game, GameMobGenerator mobGen, bool verbose)
        {
            GameEvents.Instance.ActorAdded += (g, a, align) =>
            {
                Console.WriteLine($"ActorAdded: {align}, {a}");
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
            GameEvents.Instance.AttackStart += (g, a, b) =>
            {
                Console.WriteLine($"{a.uniqueName} attacking {b.uniqueName} with {a.Weapon}");
                Console.ReadKey();
            };
            GameEvents.Instance.ActorHealthChange += (a, o, n) =>
            {
                Console.WriteLine($"{a.uniqueName} health {o} => {n}");
                Console.ReadKey();
            };
            GameEvents.Instance.Death += (g, a) =>
            {
                Console.WriteLine($"=-=-=RIP=-=-= {a.uniqueName}");
                Console.ReadKey();
            };
            if (verbose)
            {
                GameEvents.Instance.ActorTurnStart += (g, a) =>
                {
                    Console.WriteLine($"{a.uniqueName} starting turn");
                    Console.ReadKey();
                };
                GameEvents.Instance.ActorTurnEnd += (g, a) =>
                {
                    Console.WriteLine($"{a.uniqueName} ending turn");
                    Console.ReadKey();
                };
                GameEvents.Instance.AttackEnd += (g, a, b) =>
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
                    game.ClearActors(Game.ActorAlignment.Mob);
                    for (int i = 0; i < 4; ++i)
                    {
                        game.AddActor(mobGen.Gen(true), Game.ActorAlignment.Mob);
                    }
                }
                while (game.GameProgress == Game.Progress.InProgress)
                {
                    Console.WriteLine(game.ToString());
                    Console.WriteLine("Any key to continue...");
                    Console.ReadKey();

                    game.PlayRound();

                    Console.WriteLine($"Round ended with result {game.GameProgress}");
                    Console.ReadKey();
                }
            }
            Console.WriteLine($"Game ended with result {game.GameProgress}");
            GameEvents.ReleaseAllListeners();
        }
    }
}
