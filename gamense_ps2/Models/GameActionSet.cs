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
        ///     What actions are a part of this action set
        /// </summary>
        public List<GameAction> Actions { get; set; } = new();

    }

}
