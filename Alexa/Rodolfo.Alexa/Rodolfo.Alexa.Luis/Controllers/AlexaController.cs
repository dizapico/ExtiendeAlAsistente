using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rodolfo.Alexa.Luis.Contracts;
using Rodolfo.Alexa.Luis.Models;
using Rodolfo.Alexa.Luis.Services;
using Rodolfo.Alexa.Skills.Models;

namespace Rodolfo.Alexa.Luis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlexaController : ControllerBase
    {
        public IIntentFinder IntentFinder { get; private set; }

        private IStorageService storageService;

        public AlexaController(IIntentFinder intentFinder, IStorageService storageService)
        {
            this.IntentFinder = intentFinder;
            this.storageService = storageService;
        }

        // POST api/alexa
        [HttpPost]
        public async Task<SkillResponse> Post([FromBody]SkillRequest request)
        {
            var requestType = request.GetRequestType();

            if (requestType == typeof(IntentRequest))
            {
                var intentRequest = request.Request as IntentRequest;
                var phrase = intentRequest.Intent.Slots["Search"].Value;

                var intentResponse = await this.IntentFinder.GetIntentAsync(phrase);

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
                        } else
                        {
                            speechText.Append(await GetVisitingPlaceAsync(intentResponse.Entities.FirstOrDefault().Name));
                        }
                        break;
                    default:
                        speechText.Append("Rodolfo no parece que tenga ningún mensaje interesante que compartir con nosotros.");
                        break;
                }

                var speechTextString = speechText.ToString();

                return new SkillResponse
                {
                    Version = "1.0",
                    Response = new ResponseBody
                    {
                        OutputSpeech = new PlainTextOutputSpeech { Text = speechTextString },
                        Card = new SimpleCard { Title = "Rodolfo Viajero", Content = speechTextString },
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