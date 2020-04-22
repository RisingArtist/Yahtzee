using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee
{
    static class Constant
    {
        public const int NUM_OF_DIE = 5;
        
        // The random range for the dice set
        public const int DICE_SIDE_LOW_BOUND = 1;
        public const int DICE_SIDE_HIGH_BOUND = 7;

        // Game rules
        public const int TOTAL_TURNS = 13;
        public const int NUM_OF_REROLL_ALLOW = 3;

        // Keep Reroll between 0 - 4
        public const int REROLL_LOW_BOUND = -1;
        public const int REROLL_HIGH_BOUND = 5;

        // Keep picking a scorebox between -1 - 13. 
        public const int SCOREBOX_HIGH_BOUND = TOTAL_TURNS;
        public const int SCOREBOX_LOW_BOUND = REROLL_LOW_BOUND;
    }
}
