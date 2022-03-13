using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal static class Configs
    {
        public static bool Complication1 { get; set; }
        public static bool Complication2 { get; set; }
        public static bool Complication3 { get; set; }
        public static bool Complication4 { get; set; }
        public static bool Complication5 { get; set; }


        static Configs()
        {
            Complication1 = false;
            Complication2 = false;
            Complication3 = false;
            Complication4 = false;
            Complication5 = false;
        }

    }
}
