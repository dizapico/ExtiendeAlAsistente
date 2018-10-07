using Newtonsoft.Json;
using Rodolfo.Alexa.Luis.Contracts;
using Rodolfo.Alexa.Luis.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rodolfo.Alexa.Luis.Services
{
    public class LuisIntentFinder : IIntentFinder
    {
        public Uri LuisApiUri { get; private set; }
        public string LuisInstanceKey { get; private set; }

        public LuisIntentFinder(Uri luisApiUri, string luisInstanceKey)
        {
            this.LuisApiUri = luisApiUri;
            this.LuisInstanceKey = luisInstanceKey;
        }

        public async Task<IntentResponse> GetIntentAsync(string phrase)
        {
            var luisApiIntentUri = new Uri(this.LuisApiUri, $"?subscription-key={this.LuisInstanceKey}&verbose=true&timezoneOffset=0&q={phrase}");
            var luisResponse = await this.GetLuisIntent(luisApiIntentUri);

            var intentResponse = new IntentResponse();

            intentResponse.Intent = luisResponse.TopScoringIntent.Intent;
            foreach (var entity in luisResponse.Entities)
            {
                intentResponse.Entities.Add(
                                new Entity
                                {
                                    Name = entity.Entity,
                                    Score = entity.Score,
                                    StartIndex = entity.StartIndex,
                                    EndIndex = entity.EndIndex,
                                    Type = entity.Type
                                });
            }

            return intentResponse;
        }

        private async Task<LuisResponse> GetLuisIntent(Uri luisApiIntentUri)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(luisApiIntentUri);
                var luisResponse = JsonConvert.DeserializeObject<LuisResponse>(response);

                return luisResponse;
            }
        }
    }
}
