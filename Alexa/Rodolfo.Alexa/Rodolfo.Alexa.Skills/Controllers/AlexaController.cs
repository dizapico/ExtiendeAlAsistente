using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Rodolfo.Alexa.Skills.Models;
using Rodolfo.Alexa.Skills.Services;

namespace Rodolfo.Alexa.Skills.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlexaController : Controller
    {
        private IStorageService storageService;

        public AlexaController(IStorageService storageService)
        {
            this.storageService = storageService;
        }

        // POST api/alexa
        [HttpPost]
        public async Task<SkillResponse> Post([FromBody]SkillRequest request)
        {
            var requestType = request.GetRequestType();

            if (requestType == typeof(IntentRequest))
            {
                var message = string.Empty;
                var intentRequest = request.Request as IntentRequest;

                switch (intentRequest.Intent.Name)
                {
                    case "GetLastVisitingPlace":
                        message = await GetLastVisitingPlaceAsync();
                        break;
                    case "GetAllVisitingPlace":
                        message = await GetAllVisitingPlaceAsync();
                        break;
                    case "GetVisitingPlace":
                        message = await GetVisitingPlaceAsync(intentRequest.Intent.Slots.Values.FirstOrDefault().Value);
                        break;
                    default:
                        message = "Rodolfo no parece que tenga ningún mensaje interesante que compartir con nosotros.";
                        break;
                }

                return new SkillResponse
                {
                    Version = "1.0",
                    Response = new ResponseBody
                    {
                        // Use SSML here to make voice more realistic.
                        OutputSpeech = new PlainTextOutputSpeech { Text = message.ToString() },
                         Card = new SimpleCard { Title = "Rodolfo Viajero", Content = message.ToString() },
                        ShouldEndSession = true
                    }
                };
            }

            return new SkillResponse
            {
                Version = "1.0",
                Response = new ResponseBody
                {
                    // Use SSML here to make voice more realistic.
                    OutputSpeech = new PlainTextOutputSpeech { Text = $"Hmmm, Rodolfo no ha entendido tu mensaje, tal vez esté sin cobertura en algún lugar recóndito." },
                    Card = new SimpleCard { Title = "Rodolfo Viajero", Content = "Hmmm, Rodolfo no ha entendido tu mensaje, tal vez esté sin cobertura en algún lugar recóndito." },
                    ShouldEndSession = true
                }
            };
        }

        private async Task<string> GetVisitingPlaceAsync(string location)
        {
            var messages = await this.storageService.GetAllAsync();

            var message = "Rodolfo ";

            var place = messages.Where(m => m.location.ToLower().Contains(location.ToLower())).FirstOrDefault();
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
            var messages = await this.storageService.GetAllAsync();

            var places = messages.Where(m => !string.IsNullOrEmpty(m.location)).DistinctBy(m => m.location);

            var locations = places.Aggregate("", (i, j) => i + ", " + j.location);

            return $"Rodolfo ha estado en {locations}";
        }

        private async Task<string> GetLastVisitingPlaceAsync()
        {
            var messageEntity = await this.storageService.GetLastMessageAsync();

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