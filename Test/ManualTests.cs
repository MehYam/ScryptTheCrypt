using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
