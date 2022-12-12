using gamense_ps2.Code;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamense_ps2.Census {

    public class EventHandler {

        private readonly ILogger<EventHandler> _Logger;
        private readonly Vibrate _Vibrate;

        public EventHandler(ILogger<EventHandler> logger, Vibrate vibrate) {

            _Logger = logger;

            _Vibrate = vibrate;
        }


    }

}
