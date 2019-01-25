using System;

using ScryptTheCrypt;
using ScryptTheCrypt.Actions;
using ScryptTheCrypt.Utils;
namespace ManualTests
{
    class ManualTests
    {
        static void Main(string[] args)
        {
            RunSampleGame1();
        }
        static void RunSampleGame1()
        {
            Console.WriteLine("input a seed (2112): ");
            var strSeed = Console.ReadLine();
            if (!int.TryParse(strSeed, out int seed))
            {
                seed = 2112;
            }

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

            game.players.Add(player);
            game.players.Add(player2);
            game.mobs.Add(mob);
            game.mobs.Add(mob2);
            GameEvents.Instance.TurnStart += g =>
            {
                Console.WriteLine("TurnStart");
                Console.ReadKey();
            };
            GameEvents.Instance.TurnEnd += g =>
            {
                Console.WriteLine("TurnEnd");
                Console.ReadKey();
            };
            GameEvents.Instance.ActorActionsStart += (g, a) =>
            {
                Console.WriteLine($"{a.name} starting turn");
                Console.ReadKey();
            };
            GameEvents.Instance.ActorActionsEnd += (g, a) =>
            {
                Console.WriteLine($"{a.name} ending turn");
                Console.ReadKey();
            };
            GameEvents.Instance.AttackStart += (g, a, b) =>
            {
                Console.WriteLine($"{a.name} starts to attack {b.name} with {a.Weapon}");
                Console.ReadKey();
            };
            GameEvents.Instance.ActorHealthChange += (a, o, n) =>
            {
                Console.WriteLine($"{a.name} health {o} => {n}");
                Console.ReadKey();
            };
            GameEvents.Instance.AttackEnd += (g, a, b) =>
            {
                Console.WriteLine($"attack finished, target {b.name} health {b.Health}/{b.baseHealth}");
                Console.ReadKey();
            };
            GameEvents.Instance.Death += (g, a) =>
            {
                Console.WriteLine("RIP {0}", a.name);
                Console.ReadKey();
            };
            while(game.GameProgress == Game.Progress.InProgress)
            {
                Console.WriteLine(game.ToString());
                Console.WriteLine("Any key to continue...");
                Console.ReadKey();
                game.DoTurn();
            }

            Console.WriteLine("Game ended with {0}", game.GameProgress);
            GameEvents.ReleaseAllListeners();
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
    }
}
