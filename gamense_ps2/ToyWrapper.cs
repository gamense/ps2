using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamense_ps2 {

    public class ToyWrapper {

        private readonly ILogger<ToyWrapper> _Logger;

        private ButtplugClient? _Client = null;
        private ButtplugWebsocketConnector? _Connector = null;

        private double _CurrentStrength = 0d;

        public ToyWrapper(ILogger<ToyWrapper> logger) {
            _Logger = logger;

            /*
            ButtplugFFILog.SetLogOptions(ButtplugLogLevel.Trace, true);
            ButtplugFFILog.LogMessage += (obj, msg) => {
                try {
                    JToken parsed = JToken.Parse(msg);

                    string? target = parsed.Value<string?>("target");
                    string? level = parsed.GetString("level", "UNKNOWN");

                    string? message = null;

                    JToken? fields = parsed.SelectToken("fields");
                    if (fields != null) {
                        message = fields.Value<string?>("message");
                    }

                    string s = $"@{target}> {message}";

                    if (level == "TRACE") {
                        _Logger.LogTrace(s);
                    } else if (level == "DEBUG") {
                        _Logger.LogDebug(s);
                    } else if (level == "INFO") {
                        _Logger.LogInformation(s);
                    } else if (level == "WARN") {
                        _Logger.LogWarning(s);
                    } else if (level == "ERROR") {
                        _Logger.LogError(s);
                    } else {
                        _Logger.LogInformation($"Message at unknown level {level}: {s}");
                    }
                } catch (Exception ex) {
                    _Logger.LogError(ex, "failed to parse message");
                }
            };
            */

            _Connector = new ButtplugWebsocketConnector(new Uri("ws://localhost:12345/buttplug"));
            _Client = new ButtplugClient("Test client");

            _Client.DeviceAdded += (_, args) => {
                _Logger.LogInformation($"Device added {args.Device.DisplayName}/{args.Device.Name}");
                DeviceAddedEvent?.Invoke(this, args);
            };

            _Client.DeviceRemoved += (_, args) => {
                _Logger.LogInformation($"Device removed: {args.Device.DisplayName}/{args.Device.Name}");
                DeviceRemovedEvent?.Invoke(this, args);
            };
        }

        public delegate void DeviceAddedHandler(object sender, DeviceAddedEventArgs e);
        public event DeviceAddedHandler? DeviceAddedEvent;

        public delegate void DeviceRemovedHandler(object sender, DeviceRemovedEventArgs e);
        public event DeviceRemovedHandler? DeviceRemovedEvent;

        public delegate void DeviceVibrateChangeHandler(object sender, double newStrength);
        public event DeviceVibrateChangeHandler? DeviceVibrateChangeEvent;

        /// <summary>
        ///     Get a list of devices currently connected to the library client
        /// </summary>
        public List<ButtplugClientDevice> GetDevices() {
            if (_Client == null) {
                return new List<ButtplugClientDevice>();
            }

            return _Client.Devices.ToList();
        }

        /// <summary>
        ///     Get the library client
        /// </summary>
        public ButtplugClient? GetClient() {
            return _Client;
        }

        /// <summary>
        ///     Disconnect from the client
        /// </summary>
        public async Task Disconnect() {
            if (_Client == null) {
                return;
            }

            try {
                await _Client.DisconnectAsync();
            } catch (Exception ex) {
                _Logger.LogWarning(ex, $"Not connected");
            }
        }

        /// <summary>
        ///     Connect to the client
        /// </summary>
        public async Task Connect() {
            if (_Client == null) {
                return;
            }

            await _Client.ConnectAsync(_Connector);
            _Logger.LogDebug($"connected");

            await _Client.StartScanningAsync();
            _Logger.LogDebug($"starting scan");
        }

        /// <summary>
        ///     Ping all connected toys by vibrating at 20% for 1 second
        /// </summary>
        public async Task Ping() {
            if (_Client == null) {
                return;
            }

            await SetVibrate(0.2d);
            await Task.Delay(1000);
            await SetVibrate(0d);
        }

        /// <summary>
        ///     Set the vibration ratio [0-1] of all toys connected through the client
        /// </summary>
        /// <param name="strength">How strong to make the vibration as a ratio from 0 to 1 (inclusive)</param>
        public async Task SetVibrate(double strength) {
            if (_Client == null) {
                return;
            }

            await Task.WhenAll(_Client.Devices.Select(iter => {
                return iter.VibrateAsync(strength);
            }));

            _CurrentStrength = strength;
            DeviceVibrateChangeEvent?.Invoke(this, strength);
        }

        /// <summary>
        ///     Get the current vibration strength as let set by <see cref="SetVibrate(double)"/>.
        ///     If the device is being controlled by something else, the something else may
        ///     have changed the vibration strength
        /// </summary>
        public double GetStrength() {
            return _CurrentStrength;
        }

    }
}
