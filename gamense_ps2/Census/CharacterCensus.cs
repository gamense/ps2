using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using gamense_ps2.Code;
using gamense_ps2.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamense_ps2.Census {

    public class CharacterCensus {

        private readonly ILogger<CharacterCensus> _Logger;
        private readonly ICensusQueryFactory _Census;

        public CharacterCensus(ILogger<CharacterCensus> logger,
            ICensusQueryFactory census) {

            _Logger = logger;
            _Census = census;
        }

        public async Task<PsCharacter?> GetByID(string ID) {
            _Logger.LogInformation($"Getting character by ID: {ID}");

            return null;
        }

        public async Task<PsCharacter?> GetByName(string name) {
            CensusQuery query = _Census.Create("character");
            query.Where("name.first_lower").Equals(name.ToLower());

            JToken? result = null;
            try {
                result = await query.GetAsync();
            } catch (Exception ex) {
                _Logger.LogError(ex, $"error getting character with name of {name}");
            }

            _Logger.LogTrace($"{query.GetUri()} => {result}");

            if (result == null) {
                return null;
            }

            PsCharacter c = new();
            c.ID = result.GetRequiredString("character_id");

            JToken? nameField = result.SelectToken("name");
            if (nameField == null) {
                throw new Exception($"failed to find 'name' field in: {result}");
            }

            c.Name = nameField.GetRequiredString("first");

            return c;
        }

    }

}
