using gamense_ps2.Census;
using gamense_ps2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace gamense_ps2.Panels {

    /// <summary>
    /// Interaction logic for CensusPanel.xaml
    /// </summary>
    public partial class CensusPanel : UserControl {

        private readonly ILogger<CensusPanel> _Logger;
        private readonly CharacterCensus _CharacterCensus;
        private readonly RealtimeStream _RealtimeStream;
        private readonly Vibrate _Vibrate;

        private readonly Timer _Timer;

        /// <summary>
        ///     Source for the list of actions and what strength they give
        /// </summary>
        private ObservableCollection<string> _ActionListSource = new();

        private List<PsCharacter> _SubscribedCharacters = new();

        /// <summary>
        ///     Source for listing the characters subscribed to
        /// </summary>
        private ObservableCollection<string> _CharacterListSource = new();

        public CensusPanel() {
            InitializeComponent();

            _Logger = App.Services.GetRequiredService<ILogger<CensusPanel>>();
            _CharacterCensus = App.Services.GetRequiredService<CharacterCensus>();
            _RealtimeStream = App.Services.GetRequiredService<RealtimeStream>();
            _Vibrate = App.Services.GetRequiredService<Vibrate>();

            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = 1000;
            _Timer.Elapsed += _TimerElapsed;
            _Timer.Start();

            _ = UpdateActionList();
        }

        private async void Character_Input_Button_Click(object sender, RoutedEventArgs e) {
            string charName = Character_Input.Text;
            Console.WriteLine($"Adding {charName} to subscription");

            PsCharacter? c = null;
            
            try {
                c = await _CharacterCensus.GetByName(charName);
            } catch (Exception ex) {
                _Logger.LogError(ex, $"error getting {charName} from Census");
            }

            if (c == null) {
                _Logger.LogWarning($"Failed to find {charName} from Census");
                return;
            }

            _SubscribedCharacters.Add(c);

            _RealtimeStream.SubscribeToCharacterID(c.ID);
            _Logger.LogInformation($"Added {c.ID}/{c.Name} to realtime subscription");
            UpdateCharacterList();
        }

        private void Mock_Action_Button_Click(object sender, RoutedEventArgs e) {
            string action = Mock_Action.Text;
            _Vibrate.UpdateOnAction(action);
        }

        private void _TimerElapsed(object? sender, ElapsedEventArgs args) {
            Application.Current.Dispatcher.Invoke(delegate {
                Show_Strength.Text = $"Strength: {_Vibrate.GetCurrentStrength()}%";
                Show_Level.Text = $"Level: {_Vibrate.GetCurrentLevel()}";
            });
        }

        private void Action_List_Refresh_Click(object sender, RoutedEventArgs e) {
            _ = UpdateActionList();
        }

        private async Task UpdateActionList() {
            if (File.Exists("./ActionSets/Default.json") == false) {
                _Logger.LogWarning($"Failed to find './ActionSets/Default.json' in {Directory.GetCurrentDirectory()}");
                return;
            }

            string contents = await File.ReadAllTextAsync("./ActionSets/Default.json");
            _Logger.LogDebug($"Loaded {contents.Length} characters from json file");

            GameActionSet? set = JsonConvert.DeserializeObject<GameActionSet>(contents);
            if (set == null) {
                _Logger.LogWarning($"Failed to parse game action set");
                return;
            }

            _Logger.LogInformation($"Loaded action set {set.Name} with {set.Actions.Count} actions");

            List<string> feedback = new();
            HashSet<string> actions = new();
            foreach (GameAction action in set.Actions) {
                if (actions.Contains(action.Key) == true) {
                    _Logger.LogWarning($"Action '{action.Key}' is already in this set, skipping");
                    continue;
                }

                feedback.Add($"{action.Name} = {action.Value}");
                actions.Add(action.Key);

                _Logger.LogTrace($"From action set {set.Name}, action {action.Key} has value of {action.Value}; Name={action.Name}");
            }

            _Vibrate.SetActionSet(set);

            Application.Current.Dispatcher.Invoke(delegate {
                _ActionListSource.Clear();

                foreach (string f in feedback) {
                    _ActionListSource.Add(f);
                }

                Action_List.ItemsSource = _ActionListSource;
            });
        }

        private void UpdateCharacterList() {
            Application.Current.Dispatcher.Invoke(delegate {
                _CharacterListSource.Clear();

                foreach (PsCharacter c in _SubscribedCharacters) {
                    _CharacterListSource.Add(c.Name);
                }

                Character_List.ItemsSource = _CharacterListSource;
            });
        }

    }

}
