using Microsoft.Extensions.Logging;

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
