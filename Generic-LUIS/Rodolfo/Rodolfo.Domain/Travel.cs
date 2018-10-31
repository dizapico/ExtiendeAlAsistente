using Rodolfo.Cognitive;
using Rodolfo.Domain.Models;
using Rodolfo.Domain.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rodolfo.Domain
{
    public class Travel : ITravel
    {
        private IStorageService storageService;
        private readonly IIntentFinder intentFinder;

        public Travel(IStorageService storageService, IIntentFinder intentFinder)
        {
            this.storageService = storageService;
            this.intentFinder = intentFinder;
        }

        public async Task<string> EvaluateQueryText(string queryText)
        {
            var intentResponse = await this.intentFinder.GetIntentAsync(queryText);

            var speechText = new StringBuilder();
            switch (intentResponse.Intent)
            {
                case "GetLastVisitingPlace":
                    speechText.Append(await GetLastVisitingPlaceAsync());
                    break;
                case "GetAllVisitingPlace":
                    speechText.Append(await GetAllVisitingPlaceAsync());
                    break;
                case "GetVisitingPlace":
                    if (intentResponse.Entities.Count <= 0)
                    {
                        speechText.Append("No he podido encontrar ningún lugar que haya visitado Rodolfo. ");
                    }
                    else
                    {
                        speechText.Append(await GetVisitingPlaceAsync(intentResponse.Entities.FirstOrDefault().Name));
                    }
                    break;
                default:
                    speechText.Append("Rodolfo no parece que tenga ningún mensaje interesante que compartir con nosotros.");
                    break;
            }

            return speechText.ToString();
        }

        private async Task<string> GetVisitingPlaceAsync(string location)
        {
            var messages = await storageService.GetAllAsync();

            var message = "Rodolfo ";

            var place = messages.Where(m => m.location.ToLowerInvariant().Contains(location.ToLowerInvariant())).FirstOrDefault();
            if (place != null)
            {
                message += $"en {location} ha comendato que {place.caption}, pero yo en la imagen veo {place.description}";
            }
            else
            {
                message += $"no ha estado en {location}";
            }

            return message;
        }

        private async Task<string> GetAllVisitingPlaceAsync()
        {
            var messages = await storageService.GetAllAsync();

            var places = messages.Where(m => !string.IsNullOrEmpty(m.location)).DistinctBy(m => m.location);

            var locations = places.Aggregate("", (i, j) => i + ", " + j.location);

            return $"Rodolfo ha estado en {locations}";
        }

        private async Task<string> GetLastVisitingPlaceAsync()
        {
            var messageEntity = await storageService.GetLastMessageAsync();

            var message = "Rodolfo ";
            if (!string.IsNullOrEmpty(messageEntity.location))
            {
                message += $"estuvo en {messageEntity.location} ";
            }
            message += $", ha comentado que {messageEntity.caption}, pero yo en la imagen veo {messageEntity.description}";

            return message;
        }
    }
}
