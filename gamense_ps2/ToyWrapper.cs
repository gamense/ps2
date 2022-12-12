using Buttplug;
using gamense_ps2.Code;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamense_ps2 {

    public class ToyWrapper {

        private readonly ILogger<ToyWrapper> _Logger;

        private ButtplugClient? _Client = null;

        public ToyWrapper(ILogger<ToyWrapper> logger) {
            _Logger = logger;

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

            _Client = new ButtplugClient("Test client");

            _Client.DeviceAdded += (obj, args) => {
                _Logger.LogInformation($"Added {args.Device.Name}");

                string s = "";
                foreach (KeyValuePair<ServerMessage.Types.MessageAttributeType, ButtplugMessageAttributes> msg in args.Device.AllowedMessages) {
                    s += ($"{msg.Key}:{msg.Value}\n");

                    foreach (PropertyDescriptor desc in TypeDescriptor.GetProperties(msg.Value)) {
                        try {
                            s += $"\t{desc.Name}/{desc.DisplayName}: {desc.Description}\n";
                        } catch (Exception ex) {
                            s += $"\terror: {ex.Message}";
                        }
                    }
                }

                _Logger.LogDebug(s);

                DeviceAddedEvent?.Invoke(this, args);
            };

            _Client.DeviceRemoved += (obj, args) => {
                Console.WriteLine($"Removed device: {args.Device.Name}");
                DeviceRemovedEvent?.Invoke(this, args);
            };
        }

        public delegate void DeviceAddedHandler(object sender, DeviceAddedEventArgs e);
        public event DeviceAddedHandler? DeviceAddedEvent;

        public delegate void DeviceRemovedHandler(object sender, DeviceRemovedEventArgs e);
        public event DeviceRemovedHandler? DeviceRemovedEvent;

        public List<ButtplugClientDevice> GetDevices() {
            if (_Client == null) {
                return new List<ButtplugClientDevice>();
            }

            return _Client.Devices.ToList();
        }

        public ButtplugClient? GetClient() {
            return _Client;
        }

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

        public async Task Connect() {
            if (_Client == null) {
                return;
            }

            ButtplugEmbeddedConnectorOptions options = new ButtplugEmbeddedConnectorOptions();
            await _Client.ConnectAsync(options);

            await _Client.StartScanningAsync();
        }

        public async Task Ping() {
            if (_Client == null) {
                return;
            }

            await SetVibrate(0.2d);
            await Task.Delay(1000);
            await SetVibrate(0d);
        }

        public async Task SetVibrate(double strength) {
            if (_Client == null) {
                return;
            }

            await Task.WhenAll(_Client.Devices.Select(iter => {
                return iter.SendVibrateCmd(strength);
            }));
        }

    }
}
