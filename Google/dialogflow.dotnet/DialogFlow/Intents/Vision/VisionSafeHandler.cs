using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;

namespace dialogflow.dotnet.DialogFlow.Intents.Vision
{
    [Intent("vision.safe")]
    public class VisionSafeHandler : BaseVisionHandler
    {
        public VisionSafeHandler(Conversation conversation) : base(conversation)
        {
        }

        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            return new WebhookResponse { FulfillmentText = "I don't have this feature implemented, sorry for the inconvencience" };
        }    
    }
}