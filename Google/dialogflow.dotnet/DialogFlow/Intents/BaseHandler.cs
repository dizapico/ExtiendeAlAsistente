using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;

namespace dialogflow.dotnet.DialogFlow.Intents
{
    public class BaseHandler 
    {
        protected readonly Conversation conversation;


        public BaseHandler(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public virtual Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            return Task.FromResult<WebhookResponse>(null);
        }

        public virtual WebhookResponse Handle(WebhookRequest req)
        {
            return null;
        }
    }
}