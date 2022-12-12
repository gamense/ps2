using gamense_ps2.Census;
using gamense_ps2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gamense_ps2.Panels {

    /// <summary>
    /// Interaction logic for CensusPanel.xaml
    /// </summary>
    public partial class CensusPanel : UserControl {

        private readonly ILogger<CensusPanel> _Logger;
        private readonly CharacterCensus _CharacterCensus;
        private readonly RealtimeStream _RealtimeStream;
        private readonly Vibrate _Vibrate;

        public CensusPanel() {
            InitializeComponent();

            _Logger = App.Services.GetRequiredService<ILogger<CensusPanel>>();
            _CharacterCensus = App.Services.GetRequiredService<CharacterCensus>();
            _RealtimeStream = App.Services.GetRequiredService<RealtimeStream>();
            _Vibrate = App.Services.GetRequiredService<Vibrate>();
        }

        private async void Character_Input_Button_Click(object sender, RoutedEventArgs e) {
            string charName = Character_Input.Text;
            Console.WriteLine($"Adding {charName} to subscription");

            PsCharacter? c = await _CharacterCensus.GetByName(charName);

            if (c == null) {
                return;
            }

            _RealtimeStream.SubscribeToCharacterID(c.ID);
            _Logger.LogInformation($"Added {c.ID}/{c.Name} to realtime subscription");
        }

        private void Mock_Action_Button_Click(object sender, RoutedEventArgs e) {
            string action = Mock_Action.Text;
            _Vibrate.UpdateOnAction(action);
        }
    }

}
