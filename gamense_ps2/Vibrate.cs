using gamense_ps2.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace gamense_ps2 {

    public class Vibrate : IDisposable {

        private readonly ILogger<Vibrate> _Logger;
        private readonly ToyWrapper _ToyWrapper;

        private readonly Timer _Timer;
        private DateTime _LastEvent;

        private GameActionSet _ActionSet = new();
        private Dictionary<string, GameAction> _Actions { get; } = new();
        
        private int _CurrentStrength = 0;
        private int _CurrentLevel = 0;

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

            _CurrentLevel = (int)(_CurrentStrength / 100d * _ActionSet.Levels);
            if (_CurrentLevel == 0) {
                return;
            }

            double secondsAtThisLevel = _ActionSet.DecayTime / ((double)_CurrentLevel * _ActionSet.DecayFactor);

            _Logger.LogInformation($"Timer diff: {diffSpan}/{diff}; level: {_CurrentLevel}/{_ActionSet.Levels}; seconds at: {secondsAtThisLevel:2F}");

            if (diff > secondsAtThisLevel) {
                --_CurrentLevel;
                double str = _CurrentLevel / (double)_ActionSet.Levels;

                _Logger.LogInformation($"Dropped off, new str {str}");

                if (str > 1d) { str = 1d; }
                if (str < 0d) { str = 0d; }

                _ = _ToyWrapper.SetVibrate(str);
                _CurrentStrength = (int)(_CurrentLevel * (100d / _ActionSet.Levels));
                _Logger.LogTrace($"New _CurrentStrength: {_CurrentStrength}, _CurrentLevel: {_CurrentLevel}");
                _LastEvent = DateTime.UtcNow;
            }

            // get current level
            // check if we need to bump down a level
            // update level if it's changed
        }

        /// <summary>
        ///     Give (or take) points based on an action that occured
        /// </summary>
        /// <param name="eventAction">string that contains the action that occured</param>
        public async void UpdateOnAction(string eventAction) {
            _Logger.LogInformation($"Action: {eventAction}");
            if (_Actions.TryGetValue(eventAction.ToLower(), out GameAction? action) == false) {
                _Logger.LogDebug($"Action {eventAction} does not have a value, not updating vibration strength");
                return;
            }

            _CurrentStrength += action.Value;
            if (_CurrentStrength > 100) { _CurrentStrength = 100; }
            if (_CurrentStrength < 0) { _CurrentStrength = 0; }

            _Logger.LogDebug($"Action {eventAction} gives {action.Value} strength => {_CurrentStrength}, notable? {action.Notable}");

            if (action.Notable == true) {
                _LastEvent = DateTime.UtcNow;
            }

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
        ///     Get the current <see cref="GameActionSet"/> in use
        /// </summary>
        /// <returns></returns>
        public GameActionSet GetActionSet() {
            return _ActionSet;
        }

        /// <summary>
        ///     Set the active <see cref="GameActionSet"/> in use
        /// </summary>
        /// <param name="set">Action set to set</param>
        public void SetActionSet(GameActionSet set) {
            _Logger.LogInformation($"Loading set {set.Name}, {set.Actions.Count} actions defined");
            _ActionSet = set;

            _Actions.Clear();
            foreach (GameAction action in set.Actions) {
                _Actions.Add(action.Key.ToLower(), action);
            }
        }

    }
}
