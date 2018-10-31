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
        private ITravel travel;

        public AlexaController(ITravel travel)
        {
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

                var speechText = await this.travel.EvaluateQueryText(phrase);

                return new SkillResponse
                {
                    Version = "1.0",
                    Response = new ResponseBody
                    {
                        OutputSpeech = new PlainTextOutputSpeech { Text = speechText },
                        Card = new SimpleCard { Title = "Rodolfo Viajero", Content = speechText },
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
