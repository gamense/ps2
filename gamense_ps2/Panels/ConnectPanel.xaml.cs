using Buttplug;
using gamense_ps2.Census;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ConnectPanel.xaml
    /// </summary>
    public partial class ConnectPanel : UserControl {

        private readonly ILogger<ConnectPanel> _Logger;
        private readonly ToyWrapper _ToyWrapper;

        private ObservableCollection<string> _DevicesSource = new();

        public ConnectPanel() {
            InitializeComponent();

            Device_List.ItemsSource = _DevicesSource;

            _Logger = App.Services.GetRequiredService<ILogger<ConnectPanel>>();
            _ToyWrapper = App.Services.GetRequiredService<ToyWrapper>();

            UpdateDeviceList();

            _ToyWrapper.DeviceAddedEvent += _OnDeviceAdded;
            _ToyWrapper.DeviceRemovedEvent += _OnDeviceRemoved;
        }

        public void Dispose() {
            _ToyWrapper.DeviceAddedEvent -= _OnDeviceAdded;
            _ToyWrapper.DeviceRemovedEvent -= _OnDeviceRemoved;
        }

        private void _OnDeviceAdded(object sender, DeviceAddedEventArgs args) {
            _Logger.LogInformation($"New device added: {args.Device.Name}");
            UpdateDeviceList();
        }

        private void _OnDeviceRemoved(object sender, DeviceRemovedEventArgs args) {
            _Logger.LogInformation($"Device removed: {args.Device.Name}");
            UpdateDeviceList();
        }

        private void UpdateDeviceList() {
            Application.Current.Dispatcher.Invoke(delegate {
                _DevicesSource.Clear();

                foreach (ButtplugClientDevice iter in _ToyWrapper.GetDevices()) {
                    _DevicesSource.Add(iter.Name);
                }

                Device_List.ItemsSource = _DevicesSource;
            });
        }

        private async void Button_Reconnect_Click(object sender, RoutedEventArgs e) {
            _Logger.LogInformation($"Reconnect");

            await _ToyWrapper.Disconnect();
            await _ToyWrapper.Connect();

            UpdateDeviceList();
        }

        private async void Button_Ping_Click(object sender, RoutedEventArgs e) {
            await _ToyWrapper.Ping();
        }

        private void Refresh_Device_List_Click(object sender, RoutedEventArgs e) {
            UpdateDeviceList();
        }

        private async void Button_Connect_Click(object sender, RoutedEventArgs e) {
            await _ToyWrapper.Connect();
        }

        private async void Button_Disconnect_Click(object sender, RoutedEventArgs e) {
            await _ToyWrapper.Disconnect();
        }

    }
}
