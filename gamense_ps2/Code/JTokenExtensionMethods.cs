﻿using Newtonsoft.Json.Linq;
using System;

namespace gamense_ps2.Code {

    /// <summary>
    /// Helper extension methods for getting types used frequently, or specific fields used a lot
    /// </summary>
    public static class JTokenExtensionMethods {

        public static string GetRequiredString(this JToken token, string name) {
            return token.Value<string?>(name) ?? throw new ArgumentNullException($"Failed to get required field with name of '{name}', from {token}");
        }

        public static int GetRequiredInt32(this JToken token, string name) {
            return token.Value<int?>(name) ?? throw new ArgumentNullException($"Failed to get required field with name of '{name}', from {token}");
        }

        public static string GetString(this JToken token, string name, string fallback) {
            return token.Value<string?>(name) ?? fallback;
        }

        public static int GetInt32(this JToken token, string name, int fallback) {
            return token.Value<int?>(name) ?? fallback;
        }

        public static string? NullableString(this JToken token, string name) {
            return token.Value<string?>(name);
        }

        public static short GetInt16(this JToken token, string name, short fallback) {
            return token.Value<short?>(name) ?? fallback;
        }

        public static short GetRequiredInt16(this JToken token, string name) {
            return token.Value<short?>(name) ?? throw new ArgumentNullException($"Failed to get required field with name of '{name}' from {token}");
        }

        public static DateTime GetRequiredDateTime(this JToken token, string name) {
            string input = token.GetRequiredString(name);
            if (DateTime.TryParse(input, out DateTime d) == false) {
                throw new InvalidCastException($"Failed to parse {input} to a valid DateTime");
            }
            return d;
        }

        /// <summary>
        ///     Get the 'world_id' field from a JToken, or -1 if it isn't in the token
        /// </summary>
        /// <param name="token">Extension instance</param>
        /// <returns>The Int16 in the field named 'world_id', or -1 if that field doesn't exist</returns>
        public static short GetWorldID(this JToken token) {
            return token.GetInt16("world_id", -1);
        }

        public static uint GetZoneID(this JToken token) {
            return token.Value<uint?>("zone_id") ?? 0;
        }

        public static uint GetUInt32(this JToken token, string field) {
            return token.Value<uint?>(field) ?? 0;
        }

        public static bool GetBoolean(this JToken token, string name, bool fallback) {
            return token.Value<bool?>(name) ?? fallback;
        }

        public static decimal GetDecimal(this JToken token, string name, decimal fallback) {
            return token.Value<decimal?>(name) ?? fallback;
        }

        public static DateTime CensusTimestamp(this JToken token, string name) {
            return DateTimeOffset.FromUnixTimeSeconds(token.Value<int?>(name) ?? throw new ArgumentNullException(nameof(name))).UtcDateTime;
        }

    }
}
