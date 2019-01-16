using System;
using System.Collections.Generic;
using System.Text;

//KAI: should get moved to kaiGameUtils
namespace ScryptTheCrypt.Utils
{
    // Adapted from http://rosettacode.org/wiki/Subtractive_generator#C.23
    public class RNG
    {
        public const int MAX = 1000000000;
        private readonly int[] state;
        private int pos;

        private int Mod(int n)
        {
            return ((n % MAX) + MAX) % MAX;
        }
        public RNG(int seed)
        {
            state = new int[55];

            int[] temp = new int[55];
            temp[0] = Mod(seed);
            temp[1] = 1;
            for (int i = 2; i < 55; ++i)
                temp[i] = Mod(temp[i - 2] - temp[i - 1]);

            for (int i = 0; i < 55; ++i)
                state[i] = temp[(34 * (i + 1)) % 55];

            pos = 54;
            for (int i = 55; i < 220; ++i)
                Next();
        }
        public int Next()
        {
            int temp = Mod(state[(pos + 1) % 55] - state[(pos + 32) % 55]);
            pos = (pos + 1) % 55;
            state[pos] = temp;
            return temp;
        }
    }
}
