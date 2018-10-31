using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rodolfo.Cognitive;
using Rodolfo.Domain;

namespace Rodolfo.LUIS.Google.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ITravel travel;
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        public ConversationController(ITravel travel)
        {
            this.travel = travel;
        }

        [Route("Conversation")]
        [HttpPost]
        public async Task<IActionResult> Conversation()
        {
            var response = await HandleRequest(Request);
            var responseJson = response.ToString();

            return Content(responseJson, "application/json; charset=utf-8");
        }

        private async Task<WebhookResponse> HandleRequest(HttpRequest httpRequest)
        {
            WebhookRequest request;

            using (var reader = new StreamReader(httpRequest.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader);
            }

            if ((request.QueryResult != null) && (!string.IsNullOrEmpty(request.QueryResult.QueryText))) 
            {
                var speechText = await travel.EvaluateQueryText(request.QueryResult.QueryText);

                return new WebhookResponse
                {
                    FulfillmentText = speechText
                };
            }

            return new WebhookResponse
            {
                FulfillmentText = $"Hmmm, Rodolfo no ha entendido tu mensaje, tal vez esté sin cobertura en algún lugar recóndito."
            };
        }
    }
}
