using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Rodolfo.Cognitive;
using Rodolfo.Domain;

namespace Rodolfo.LUIS.Alexa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlexaController : ControllerBase
    {
        public IIntentFinder IntentFinder { get; private set; }

        private ITravel travel;

        public AlexaController(IIntentFinder intentFinder, ITravel travel)
        {
            this.IntentFinder = intentFinder;
            this.travel = travel;
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
                        speechText.Append(await travel.GetLastVisitingPlaceAsync());
                        break;
                    case "GetAllVisitingPlace":
                        speechText.Append(await travel.GetAllVisitingPlaceAsync());
                        break;
                    case "GetVisitingPlace":
                        if (intentResponse.Entities.Count <= 0)
                        {
                            speechText.Append("No he podido encontrar ningún lugar que haya visitado Rodolfo. ");
                        }
                        else
                        {
                            speechText.Append(await travel.GetVisitingPlaceAsync(intentResponse.Entities.FirstOrDefault().Name));
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
    }
}
