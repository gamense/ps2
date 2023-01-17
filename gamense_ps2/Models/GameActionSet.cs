using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamense_ps2.Models {

    /// <summary>
    ///     Represents a collection of <see cref="GameAction"/>
    /// </summary>
    public class GameActionSet {

        /// <summary>
        ///     Display friendly name of the action set
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        ///     How many discrete levels of vibration there will be
        /// </summary>
        public int Levels { get; set; } = 5;

        /// <summary>
        ///     How many seconds it will take to go from level 1 to level 0
        /// </summary>
        public int DecayTime { get; set; } = 30;

        /// <summary>
        ///     A scaling factor to <see cref="DecayTime"/>. Set less than 1 (but greater than 0)
        ///     to increase the time between level drops
        /// </summary>
        public double DecayFactor { get; set; } = 1d;

        /// <summary>
        ///     What actions are a part of this action set
        /// </summary>
        public List<GameAction> Actions { get; set; } = new();

    }

}
