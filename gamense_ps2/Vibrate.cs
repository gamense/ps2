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
        private int _CurrentLevel = 0;

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
        }

        public void Dispose() {
            _Timer.Elapsed -= _TimerElapsed;
        }

        private void _TimerElapsed(object? sender, ElapsedEventArgs args) {
            TimeSpan diffSpan = args.SignalTime.ToUniversalTime() - _LastEvent;
            int diff = (int)diffSpan.TotalSeconds;

            _CurrentLevel = (int)(_CurrentStrength / 100d * Divisions);
            if (_CurrentLevel == 0) {
                return;
            }

            int secondsAtThisLevel = SECONDS_PER_LEVEL / _CurrentLevel;

            _Logger.LogInformation($"Timer diff: {diffSpan}/{diff}; level: {_CurrentLevel}/{Divisions}; seconds at: {secondsAtThisLevel}");

            if (diff > secondsAtThisLevel) {
                --_CurrentLevel;
                double str = _CurrentLevel / (double)Divisions;

                _Logger.LogInformation($"Dropped off, new str {str}");

                if (str > 1d) { str = 1d; }
                if (str < 0d) { str = 0d; }

                _ = _ToyWrapper.SetVibrate(str);
                _CurrentStrength = (int)(_CurrentLevel * (100d / Divisions));
                _Logger.LogTrace($"New _CurrentStrength: {_CurrentStrength}, _CurrentLevel: {_CurrentLevel}");
                _LastEvent = DateTime.UtcNow;
            }

            // get current level
            // check if we need to bump down a level
            // update level if it's changed
        }

        /// <summary>
        ///     Set how much % this action done will grant (or take away if negative)
        /// </summary>
        /// <param name="action">Key of the action</param>
        /// <param name="strength">Value from 0 to 100 representing how much percentage strength this action will grant</param>
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

        /// <summary>
        ///     Clear all the actions and their associated strengths
        /// </summary>
        public void ClearActionStrengths() {
            Actions.Clear();
        }

        public async void UpdateOnAction(string action) {
            _Logger.LogInformation($"Action: {action}");
            if (Actions.TryGetValue(action.ToLower(), out int str) == false) {
                _Logger.LogTrace($"Action {action} does not have a value, not updating vibration strength");
                return;
            }

            _CurrentStrength += str;
            if (_CurrentStrength > 100) { _CurrentStrength = 100; }
            if (_CurrentStrength < 0) { _CurrentStrength = 0; }

            _Logger.LogDebug($"Action {action} gives {str} strength => {_CurrentStrength}");

            _LastEvent = DateTime.UtcNow;

            double newStr = _CurrentStrength / 100d;
            await _ToyWrapper.SetVibrate(newStr);
        }

        /// <summary>
        ///     Get the value 0-100 that represents the current strength of the toy.
        /// </summary>
        public int GetCurrentStrength() {
            return _CurrentStrength;
        }

        /// <summary>
        ///     Get the current vibration level
        /// </summary>
        public int GetCurrentLevel() {
            return _CurrentLevel;
        }

        /// <summary>
        ///     Get how many different vibration levels there are
        /// </summary>
        public int GetMaxLevel() {
            return Divisions;
        }

    }
}
