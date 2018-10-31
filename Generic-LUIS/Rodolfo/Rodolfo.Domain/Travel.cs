using Rodolfo.Domain.Models;
using Rodolfo.Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rodolfo.Domain
{
    public class Travel : ITravel
    {
        private IStorageService storageService;

        public Travel(IStorageService storageService)
        {
            this.storageService = storageService;
        }

        public async Task<string> GetVisitingPlaceAsync(string location)
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

        public async Task<string> GetAllVisitingPlaceAsync()
        {
            var messages = await storageService.GetAllAsync();

            var places = messages.Where(m => !string.IsNullOrEmpty(m.location)).DistinctBy(m => m.location);

            var locations = places.Aggregate("", (i, j) => i + ", " + j.location);

            return $"Rodolfo ha estado en {locations}";
        }

        public async Task<string> GetLastVisitingPlaceAsync()
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
