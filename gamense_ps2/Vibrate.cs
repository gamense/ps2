using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace gamense_ps2 {

    public class Vibrate : IDisposable {

        public int Divisions { get; set; } = 5;

        public Dictionary<string, int> Actions { get; } = new();

        private readonly ILogger<Vibrate> _Logger;
        private readonly ToyWrapper _ToyWrapper;

        private readonly Timer _Timer;
        private DateTime _LastEvent;

        private int _CurrentStrength = 0;

        private const int SECONDS_PER_LEVEL = 30;

        public Vibrate(ILogger<Vibrate> logger, ToyWrapper toyWrapper) {
            _Logger = logger;
            _ToyWrapper = toyWrapper;

            _LastEvent = DateTime.UtcNow;

            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = 1000;
            _Timer.Elapsed += _TimerElapsed;
            _Timer.Start();

            SetActionStrength("Kill", 20);
            SetActionStrength("Death", -5);
        }

        public void Dispose() {
            _Timer.Elapsed -= _TimerElapsed;
        }

        private void _TimerElapsed(object? sender, ElapsedEventArgs args) {
            TimeSpan diffSpan = args.SignalTime.ToUniversalTime() - _LastEvent;
            int diff = (int)diffSpan.TotalSeconds;

            //int currentLevel = _CurrentStrength / Math.Max(1, Divisions);
            int currentLevel = (int)(_CurrentStrength / 100d * Divisions);
            if (currentLevel == 0) {
                return;
            }

            int secondsAtThisLevel = SECONDS_PER_LEVEL / currentLevel;

            _Logger.LogInformation($"Timer diff: {diffSpan}/{diff}; level: {currentLevel}/{Divisions}; seconds at: {secondsAtThisLevel}");

            if (diff > secondsAtThisLevel) {
                --currentLevel;
                double str = currentLevel / (double)Divisions;

                _Logger.LogInformation($"Dropped off, new str {str}");

                if (str > 1d) { str = 1d; }
                if (str < 0d) { str = 0d; }

                _ = _ToyWrapper.SetVibrate(str);
                _CurrentStrength = (int)(currentLevel * (100d / Divisions));
                _Logger.LogInformation($"New _CurrentStrength: {_CurrentStrength}");
                _LastEvent = DateTime.UtcNow;
            }

            // get current level
            // check if we need to bump down a level
            // update level if it's changed
        }

        public void SetActionStrength(string action, int strength) {
            action = action.ToLower();

            // ensure safe bounds
            if (strength > 100) { strength = 100; }

            if (Actions.ContainsKey(action)) {
                Actions[action] = strength;
            } else {
                Actions.Add(action, strength);
            }
        }

        public async void UpdateOnAction(string action) {
            _Logger.LogInformation($"Action: {action}");
            if (Actions.TryGetValue(action.ToLower(), out int str) == false) {
                return;
            }

            _CurrentStrength += str;
            if (_CurrentStrength > 100) { _CurrentStrength = 100; }
            if (_CurrentStrength < 0) { _CurrentStrength = 0; }

            _Logger.LogInformation($"Action {action} gives {str} strength => {_CurrentStrength}");

            _LastEvent = DateTime.UtcNow;

            double newStr = _CurrentStrength / 100d;
            await _ToyWrapper.SetVibrate(newStr);
        }

    }
}
