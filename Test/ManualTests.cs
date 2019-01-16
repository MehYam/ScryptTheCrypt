using System;

using ScryptTheCrypt;
using ScryptTheCrypt.Utils;
namespace Test
{
    class ManualTests
    {
        static void Main(string[] args)
        {
            RunSampleGame();
        }
        static void RunSampleGame()
        {
            var game = new Game(2112);
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
