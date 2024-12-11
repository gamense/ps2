using DaybreakGames.Census.Stream;
using gamense_ps2.Code;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Websocket.Client;

namespace gamense_ps2.Census {

    public class RealtimeStream {

        private readonly ILogger<RealtimeStream> _Logger;
        private readonly ICensusStreamClient _Realtime;
        private readonly Vibrate _Vibrate;

        private List<string> _TrackedCharacters = new();

        public RealtimeStream(ILogger<RealtimeStream> logger,
            ICensusStreamClient realtime, Vibrate vibrate) {

            _Logger = logger;
            _Vibrate = vibrate;

            _Realtime = realtime;
            _Realtime.OnConnect(_OnConnect);
            _Realtime.OnMessage(_OnMessage);
            _Realtime.OnDisconnect(_OnDisconnect);

            _ = _Realtime.ConnectAsync();
        }

        public async Task Connect() {
            _Realtime.SetServiceId("gamense");
            _Realtime.SetServiceNamespace("pc");
            await _Realtime.ConnectAsync();
            _Logger.LogInformation($"Realtime stream connected");
        }

        public async Task Reconnect() {
            await Disconnect();
            await Connect();
        }

        public async Task Disconnect() {
            await _Realtime.DisconnectAsync();
        }

        private Task _OnConnect(ReconnectionType type) {
            _Logger.LogInformation($"Connected: {type}");
            return Task.CompletedTask;
        }

        private async Task _OnMessage(string msg) {
            try {
                JToken token = JToken.Parse(msg);

                string? type = token.Value<string?>("type");
                if (type == "serviceMessage") {
                    JToken? payload = token.SelectToken("payload");
                    if (payload == null) {
                        return;
                    }

                    await ProcessEvent(payload);
                } else if (type == "heartbeat") {

                }

            } catch (Exception ex) {
                _Logger.LogError(ex, $"failed to parse message: {msg}");
            }
        }

        private Task _OnDisconnect(DisconnectionInfo info) {
            _Logger.LogInformation($"Disconnect: {info.Type}");
            return Task.CompletedTask;
        }

        public void Subscribe(CensusStreamSubscription sub) {
            _Realtime.Subscribe(sub);
        }

        public void SubscribeToCharacterID(string ID) {
            CensusStreamSubscription sub = new() {
                Characters = new[] { ID },
                EventNames = new[] { "Death", "GainExperience", "PlayerLogin", "PlayerLogout" }
            };

            Subscribe(sub);

            _TrackedCharacters.Add(ID);
        }

        private Task ProcessEvent(JToken token) {
            _Logger.LogTrace($"Event: {token}");

            string? eventName = token.Value<string?>("event_name");

            if (eventName == null) {
                _Logger.LogWarning($"Missing 'event_name' from: {token}");
            } else if (eventName == "Death") {
                _Logger.LogTrace($"Death: {token}");

                string killed = token.GetString("character_id", "0");
                string attacker = token.GetString("attacker_character_id", "0");

                if (_TrackedCharacters.Contains(attacker)) {
                    _Vibrate.UpdateOnAction("Kill");
                }
                if (_TrackedCharacters.Contains(killed)) {
                    _Vibrate.UpdateOnAction("Death");
                }

            } else if (eventName == "GainExperience") {
                _Logger.LogTrace($"GainExperience: {token}");

                string source = token.GetString("character_id", "0");
                string other = token.GetString("other_id", "0");

                int experienceID = token.GetInt32("experience_id", 0);

                if (_TrackedCharacters.Contains(source)) {
                    _Vibrate.UpdateOnAction($"GainExperience.Source.{experienceID}");
                }
                if (_TrackedCharacters.Contains(other)) {
                    _Vibrate.UpdateOnAction($"GainExperience.Other.{experienceID}");
                }
            } else if (eventName == "PlayerLogin") {

            } else if (eventName == "PlayerLogout") {

            } else {
                _Logger.LogWarning($"Unchecked event_name: '{eventName}");
            }

            return Task.CompletedTask;
        }

    }
}
