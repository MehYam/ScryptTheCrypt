using System.Collections.Generic;
using kaiGameUtil;

namespace ScryptTheCrypt
{
    public sealed class Game
    {
        public readonly RNG rng;
        public List<GameActor> players = new List<GameActor>();

        public GameBattle CurrentBattle { get; private set; }
        public Game(int seed = 0)
        {
            rng = new RNG(seed);
        }
        public void Start(int level = 0)
        {
            CurrentBattle = new GameBattle(rng);
            CurrentBattle.players.InsertRange(0, players);
        }
    }
}
