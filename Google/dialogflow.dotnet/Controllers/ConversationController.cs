using System.Threading.Tasks;
using dialogflow.dotnet.DialogFlow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dialogflow.dotnet.Controllers 
{
    public class ConversationController : Controller
    {
        private readonly DialogflowApp dialogFlowApp;

        public ConversationController(ILogger<ConversationController> logger)
        {
            dialogFlowApp = new DialogflowApp(logger);
        }

        [Route("Conversation")]
        [HttpPost]
        public async Task<IActionResult> Conversation()
        {
            var response = await dialogFlowApp.HandleRequest(Request);
            var responseJson = response.ToString();

            return Content(responseJson, "application/json; charset=utf-8");
        }
    }
}