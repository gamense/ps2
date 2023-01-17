using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamense_ps2.Models {

    /// <summary>
    ///     Represents an event that changes the current strength of the connected toys
    /// </summary>
    public class GameAction {

        /// <summary>
        ///     Display friendly name of the action
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        ///     What key is used internally
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        ///     How much strength this action will add. While any integer can be used, it is capped at 100 internally.
        ///     Use negative values to take away strength
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        ///     Will this action reset the decay timer?
        /// </summary>
        public bool Notable { get; set; } = false;

    }
}
