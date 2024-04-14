using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Crypt
{
    public class KeyRand
    {
        public UInt32 Seed;

        public KeyRand()
        {
            Seed = 0;
        }

        public Int64 Next()
        {
            Seed = Seed * 49723125 + 21403831;
            return ((Int64)Seed >> 16) * 41894339 + 11741117 >> 16 & 0xFFFF;
        }
    }
}
